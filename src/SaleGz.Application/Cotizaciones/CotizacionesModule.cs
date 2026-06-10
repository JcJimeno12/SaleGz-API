using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Cotizaciones;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record CotizacionDetalleDto(int CotizacionDetalleId, int ProductoId, string Producto, decimal Cantidad, decimal SubTotal, decimal ITBIS);
public record CotizacionDto(int CotizacionId, int? ClienteId, string? NombreCliente, int UsuarioId, string Usuario, decimal Total, decimal ITBIS, DateTime Fecha, List<CotizacionDetalleDto> Detalles);
public record CotizacionListDto(int CotizacionId, string? NombreCliente, decimal Total, DateTime Fecha);
public record CotizacionDetalleRequest(int ProductoId, decimal Cantidad, decimal SubTotal, decimal ITBIS);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetCotizacionesQuery : IRequest<List<CotizacionListDto>>;

public class GetCotizacionesHandler : IRequestHandler<GetCotizacionesQuery, List<CotizacionListDto>>
{
    private readonly IAppDbContext _context;
    public GetCotizacionesHandler(IAppDbContext context) => _context = context;

    public async Task<List<CotizacionListDto>> Handle(GetCotizacionesQuery request, CancellationToken ct)
        => await _context.Cotizaciones
            .Include(c => c.Cliente)
            .OrderByDescending(c => c.Fecha)
            .Select(c => new CotizacionListDto(
                c.CotizacionId, c.Cliente != null ? c.Cliente.Nombre : null, c.Total, c.Fecha))
            .ToListAsync(ct);
}

public record GetCotizacionByIdQuery(int Id) : IRequest<CotizacionDto>;

public class GetCotizacionByIdHandler : IRequestHandler<GetCotizacionByIdQuery, CotizacionDto>
{
    private readonly IAppDbContext _context;
    public GetCotizacionByIdHandler(IAppDbContext context) => _context = context;

    public async Task<CotizacionDto> Handle(GetCotizacionByIdQuery request, CancellationToken ct)
    {
        var c = await _context.Cotizaciones
            .Include(x => x.Detalles).ThenInclude(d => d.Producto)
            .Include(x => x.Cliente)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.CotizacionId == request.Id, ct)
            ?? throw new NotFoundException(nameof(Cotizacion), request.Id);

        return new CotizacionDto(c.CotizacionId, c.ClienteId, c.Cliente?.Nombre, c.UsuarioId, c.Usuario.Nombre,
            c.Total, c.ITBIS, c.Fecha,
            c.Detalles.Select(d => new CotizacionDetalleDto(
                d.CotizacionDetalleId, d.ProductoId, d.Producto.Descripcion,
                d.Cantidad, d.SubTotal, d.ITBIS)).ToList());
    }
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearCotizacionCommand(
    int? ClienteId,
    int UsuarioId,
    decimal Total,
    decimal ITBIS,
    List<CotizacionDetalleRequest> Detalles
) : IRequest<int>;

public class CrearCotizacionValidator : AbstractValidator<CrearCotizacionCommand>
{
    public CrearCotizacionValidator()
    {
        RuleFor(x => x.Total).GreaterThan(0);
        RuleFor(x => x.Detalles).NotEmpty();
    }
}

public class CrearCotizacionHandler : IRequestHandler<CrearCotizacionCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearCotizacionHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearCotizacionCommand request, CancellationToken ct)
    {
        var cotizacion = new Cotizacion
        {
            ClienteId = request.ClienteId,
            UsuarioId = request.UsuarioId,
            Total = request.Total,
            ITBIS = request.ITBIS,
            Fecha = DateTime.Now
        };

        foreach (var d in request.Detalles)
        {
            cotizacion.Detalles.Add(new CotizacionDetalle
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                SubTotal = d.SubTotal,
                ITBIS = d.ITBIS
            });
        }

        _context.Cotizaciones.Add(cotizacion);
        await _context.SaveChangesAsync(ct);
        return cotizacion.CotizacionId;
    }
}

// ── CONVERTIR COTIZACIÓN A FACTURA ──────
public record ConvertirCotizacionCommand(
    int CotizacionId, 
    string TipoPago,
    decimal Pagado,
    bool Tarjeta,
    int TipoComprobante,
    int TipoFactura
) : IRequest<int>;

public class ConvertirCotizacionHandler : IRequestHandler<ConvertirCotizacionCommand, int>
{
    private readonly IMediator _mediator;
    private readonly IAppDbContext _context;

    public ConvertirCotizacionHandler(IMediator mediator, IAppDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task<int> Handle(ConvertirCotizacionCommand request, CancellationToken ct)
    {
        var cotizacion = await _context.Cotizaciones
            .Include(c => c.Detalles)
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.CotizacionId == request.CotizacionId, ct)
            ?? throw new NotFoundException(nameof(Cotizacion), request.CotizacionId);

        var nombreCliente = cotizacion.Cliente?.Nombre ?? "Consumidor Final";

        var detalles = cotizacion.Detalles.Select(d => new Facturas.FacturaDetalleRequest(
            d.ProductoId, d.Cantidad, d.SubTotal, d.ITBIS, 0, 0)).ToList();

        var facturaId = await _mediator.Send(new Facturas.CrearFacturaCommand( 
    ClienteId: cotizacion.ClienteId,
    UsuarioId: cotizacion.UsuarioId,
    NombreCliente: nombreCliente, 
    TipoPago: request.TipoPago,
    Total: cotizacion.Total,
    ITBS: cotizacion.ITBIS,
    Pagado: request.Pagado,
    Exento: 0,
    Gravado: cotizacion.Total,
    Tarjeta: request.Tarjeta, 
    TipoComprobante: request.TipoComprobante,
     TipoFactura: request.TipoFactura,
    Detalles: detalles
), ct);
        return facturaId;
    }
}

public record EliminarCotizacionCommand(int CotizacionId) : IRequest;

public class EliminarCotizacionHandler : IRequestHandler<EliminarCotizacionCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public EliminarCotizacionHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(EliminarCotizacionCommand request, CancellationToken ct)
    {
        if (_currentUser.TipoUsuario != 0)  // 0 = Admin
            throw new ForbiddenException();

        var cotizacion = await _context.Cotizaciones.FindAsync([request.CotizacionId], ct)
            ?? throw new NotFoundException(nameof(Cotizacion), request.CotizacionId);

        _context.Cotizaciones.Remove(cotizacion);
        await _context.SaveChangesAsync(ct);
    }
}


public record ActualizarCotizacionCommand(
    int CotizacionId,
    decimal Total,
    decimal ITBIS,
    List<CotizacionDetalleRequest> Detalles
) : IRequest;

public class ActualizarCotizacionValidator : AbstractValidator<ActualizarCotizacionCommand>
{
    public ActualizarCotizacionValidator()
    {
        RuleFor(x => x.CotizacionId).GreaterThan(0);
        RuleFor(x => x.Total).GreaterThan(0);
        RuleFor(x => x.Detalles).NotEmpty();
    }
}
public class ActualizarCotizacionHandler : IRequestHandler<ActualizarCotizacionCommand>
{
    private readonly IAppDbContext _context;

    public ActualizarCotizacionHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarCotizacionCommand request, CancellationToken ct)
    {
        var cotizacion = await _context.Cotizaciones
            .Include(c => c.Detalles)
            .FirstOrDefaultAsync(c => c.CotizacionId == request.CotizacionId, ct)
            ?? throw new NotFoundException(nameof(Cotizacion), request.CotizacionId);

        // Actualizar cabecera 
        cotizacion.Total = request.Total;
        cotizacion.ITBIS = request.ITBIS;

        // 🔥 Eliminar detalles actuales
        _context.CotizacionDetalles.RemoveRange(cotizacion.Detalles);

        // 🔥 Agregar nuevos
        cotizacion.Detalles = request.Detalles.Select(d => new CotizacionDetalle
        {
            ProductoId = d.ProductoId,
            Cantidad = d.Cantidad,
            SubTotal = d.SubTotal,
            ITBIS = d.ITBIS
        }).ToList();

        await _context.SaveChangesAsync(ct);
    }
}