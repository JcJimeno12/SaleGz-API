using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Compras;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record CompraDetalleDto(int CompraDetalleId, int ProductoId, string Producto, decimal Cantidad, decimal SubTotal);

public record CompraDto(
    int CompraId,
    int? ClienteId,
    string? NombreCliente,
    int UsuarioId,
    string Usuario,
    decimal Total,
    DateTime Fecha,
    string TipoPago,
    int Estado,
    List<CompraDetalleDto> Detalles
);

public record CompraListDto(int CompraId, string? NombreCliente, decimal Total, DateTime Fecha, string TipoPago, int Estado);
public record CompraDetalleRequest(int ProductoId, decimal Cantidad, decimal SubTotal);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetComprasQuery(int? Estado = null) : IRequest<List<CompraListDto>>;

public class GetComprasHandler : IRequestHandler<GetComprasQuery, List<CompraListDto>>
{
    private readonly IAppDbContext _context;
    public GetComprasHandler(IAppDbContext context) => _context = context;

    public async Task<List<CompraListDto>> Handle(GetComprasQuery request, CancellationToken ct)
    {
        var query = _context.Compras.Include(c => c.Cliente).AsQueryable();

        if (request.Estado.HasValue)
            query = query.Where(c => (int)c.Estado == request.Estado.Value);

        return await query
            .OrderByDescending(c => c.Fecha)
            .Select(c => new CompraListDto(
                c.CompraId, c.Cliente != null ? c.Cliente.Nombre : null,
                c.Total, c.Fecha, c.TipoPago, (int)c.Estado))
            .ToListAsync(ct);
    }
}

public record GetCompraByIdQuery(int Id) : IRequest<CompraDto>;

public class GetCompraByIdHandler : IRequestHandler<GetCompraByIdQuery, CompraDto>
{
    private readonly IAppDbContext _context;
    public GetCompraByIdHandler(IAppDbContext context) => _context = context;

    public async Task<CompraDto> Handle(GetCompraByIdQuery request, CancellationToken ct)
    {
        var c = await _context.Compras
            .Include(x => x.Detalles).ThenInclude(d => d.Producto)
            .Include(x => x.Cliente)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.CompraId == request.Id, ct)
            ?? throw new NotFoundException(nameof(Compra), request.Id);

        return new CompraDto(c.CompraId, c.ClienteId, c.Cliente?.Nombre, c.UsuarioId, c.Usuario.Nombre,
            c.Total, c.Fecha, c.TipoPago, (int)c.Estado,
            c.Detalles.Select(d => new CompraDetalleDto(
                d.CompraDetalleId, d.ProductoId, d.Producto.Descripcion, d.Cantidad, d.SubTotal)).ToList());
    }
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearCompraCommand(
    int? ClienteId,
    int UsuarioId,
    decimal Total,
    string TipoPago,
    int Estado,
    List<CompraDetalleRequest> Detalles
) : IRequest<int>;

public class CrearCompraValidator : AbstractValidator<CrearCompraCommand>
{
    public CrearCompraValidator()
    {
        RuleFor(x => x.Total).GreaterThan(0);
        RuleFor(x => x.TipoPago).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("La compra debe tener al menos un producto.");
    }
}

public class CrearCompraHandler : IRequestHandler<CrearCompraCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearCompraHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearCompraCommand request, CancellationToken ct)
    {
        var compra = new Compra
        {
            ClienteId = request.ClienteId,
            UsuarioId = request.UsuarioId,
            Total     = request.Total,
            TipoPago  = request.TipoPago,
            Estado    = (EstadoCompra)request.Estado,
            Fecha     = DateTime.Now
        };

        foreach (var d in request.Detalles)
        {
            compra.Detalles.Add(new CompraDetalle
            {
                ProductoId = d.ProductoId,
                Cantidad   = d.Cantidad,
                SubTotal   = d.SubTotal
            });

            // Aumentar inventario
            var producto = await _context.Productos.FindAsync([d.ProductoId], ct);
            if (producto != null)
                producto.Cantidad += d.Cantidad;
        }

        _context.Compras.Add(compra);
        await _context.SaveChangesAsync(ct);

        // Si es crédito registrar cuenta por pagar
        if (request.Estado == (int)EstadoCompra.Credito)
        {
            _context.CuentasPorPagar.Add(new CuentaPorPagarDetalle
            {
                CompraId  = compra.CompraId,
                UsuarioId = request.UsuarioId,
                Monto     = request.Total,
                Fecha     = DateTime.Now
            });
            await _context.SaveChangesAsync(ct);
        }

        return compra.CompraId;
    }
}

public record SaldarCompraCommand(int CompraId, int UsuarioId, decimal Monto) : IRequest;

public class SaldarCompraValidator : AbstractValidator<SaldarCompraCommand>
{
    public SaldarCompraValidator()
    {
        RuleFor(x => x.Monto).GreaterThan(0);
    }
}

public class SaldarCompraHandler : IRequestHandler<SaldarCompraCommand>
{
    private readonly IAppDbContext _context;
    public SaldarCompraHandler(IAppDbContext context) => _context = context;

    public async Task Handle(SaldarCompraCommand request, CancellationToken ct)
    {
        var compra = await _context.Compras.FindAsync([request.CompraId], ct)
            ?? throw new NotFoundException(nameof(Compra), request.CompraId);

        if (compra.Estado == EstadoCompra.Saldado)
            throw new BusinessException("La compra ya está saldada.");

        if (compra.Estado == EstadoCompra.Cancelado)
            throw new BusinessException("No se puede saldar una compra cancelada.");

        _context.CuentasPorPagar.Add(new CuentaPorPagarDetalle
        {
            CompraId  = compra.CompraId,
            UsuarioId = request.UsuarioId,
            Monto     = request.Monto,
            Fecha     = DateTime.Now
        });

        var totalPagado = await _context.CuentasPorPagar
            .Where(c => c.CompraId == compra.CompraId)
            .SumAsync(c => c.Monto, ct);

        if (totalPagado >= compra.Total)
            compra.Estado = EstadoCompra.Saldado;

        await _context.SaveChangesAsync(ct);
    }
}

public record CancelarCompraCommand(int CompraId) : IRequest;

public class CancelarCompraHandler : IRequestHandler<CancelarCompraCommand>
{
    private readonly IAppDbContext _context;
    public CancelarCompraHandler(IAppDbContext context) => _context = context;

    public async Task Handle(CancelarCompraCommand request, CancellationToken ct)
    {
        var compra = await _context.Compras
            .Include(c => c.Detalles)
            .FirstOrDefaultAsync(c => c.CompraId == request.CompraId, ct)
            ?? throw new NotFoundException(nameof(Compra), request.CompraId);

        if (compra.Estado == EstadoCompra.Cancelado)
            throw new BusinessException("La compra ya está cancelada.");

        foreach (var detalle in compra.Detalles)
        {
            var producto = await _context.Productos.FindAsync([detalle.ProductoId], ct);
            if (producto != null)
                producto.Cantidad -= detalle.Cantidad;
        }

        compra.Estado = EstadoCompra.Cancelado;
        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarCompraCommand(int CompraId) : IRequest;

public class EliminarCompraHandler : IRequestHandler<EliminarCompraCommand>
{
    private readonly IAppDbContext _context;
    public EliminarCompraHandler(IAppDbContext context) => _context = context;

    public async Task Handle(EliminarCompraCommand request, CancellationToken ct)
    {
        var compra = await _context.Compras.FindAsync([request.CompraId], ct)
            ?? throw new NotFoundException(nameof(Compra), request.CompraId);

        _context.Compras.Remove(compra);
        await _context.SaveChangesAsync(ct);
    }
}
