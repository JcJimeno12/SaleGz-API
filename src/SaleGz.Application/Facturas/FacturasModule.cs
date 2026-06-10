using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Facturas;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record FacturaDetalleDto(
    int FacturaDetalleId,
    int ProductoId,
    string Producto,
    decimal Cantidad,
    decimal SubTotal,
    decimal ITBS,
    decimal Costo,
    decimal Comision
);

public record FacturaDto(
    int FacturaId,
    int? ClienteId,
    string NombreCliente,
    int UsuarioId,
    string Usuario,
    string TipoPago,
    decimal Total,
    decimal ITBS,
    decimal Pagado,
    decimal Exento,
    decimal Gravado,
    DateTime Fecha,
    int Estado,
    bool Tarjeta,
    int TipoComprobante,
    int TipoFactura,
    List<FacturaDetalleDto> Detalles
);

public record FacturaListDto(
    int FacturaId,
    string NombreCliente,
    decimal Total,
    decimal Pagado,
    string TipoPago,
    DateTime Fecha,
    int Estado,
    int TipoFactura
);
 

public record FacturaDetalleRequest(
    int ProductoId,
    decimal Cantidad,
    decimal SubTotal,
    decimal ITBS,
    decimal Costo,
    decimal Comision
);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetFacturasQuery(int? Estado = null, DateTime? Desde = null, DateTime? Hasta = null, int? UsuarioId = null, int? TipoComprobante = null)
    : IRequest<List<FacturaListDto>>;

public class GetFacturasHandler : IRequestHandler<GetFacturasQuery, List<FacturaListDto>>
{
    private readonly IAppDbContext _context;
    public GetFacturasHandler(IAppDbContext context) => _context = context;

    public async Task<List<FacturaListDto>> Handle(GetFacturasQuery request, CancellationToken ct)
    {
        var query = _context.Facturas.AsQueryable();

        if (request.Estado.HasValue)
            query = query.Where(f => (int)f.Estado == request.Estado.Value);

        if (request.Desde.HasValue)
            query = query.Where(f => f.Fecha >= request.Desde.Value.Date);

        if (request.Hasta.HasValue)
            query = query.Where(f => f.Fecha <= request.Hasta.Value.Date.AddDays(1).AddTicks(-1));

        if (request.UsuarioId.HasValue)
            query = query.Where(f => f.UsuarioId == request.UsuarioId.Value);

        if (request.TipoComprobante.HasValue)
            query = query.Where(f => f.TipoComprobante == request.TipoComprobante.Value);
         
        return await query
            .OrderByDescending(f => f.Fecha)
            .Select(f => new FacturaListDto(
                f.FacturaId, f.NombreCliente, f.Total, f.Pagado,
                f.TipoPago, f.Fecha, (int)f.Estado, (int)f.TipoFactura))
            .ToListAsync(ct);
    }
}

public record GetFacturaByIdQuery(int Id) : IRequest<FacturaDto>;

public class GetFacturaByIdHandler : IRequestHandler<GetFacturaByIdQuery, FacturaDto>
{
    private readonly IAppDbContext _context;
    public GetFacturaByIdHandler(IAppDbContext context) => _context = context;

    public async Task<FacturaDto> Handle(GetFacturaByIdQuery request, CancellationToken ct)
    {
        var f = await _context.Facturas
            .Include(x => x.Detalles).ThenInclude(d => d.Producto)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.FacturaId == request.Id, ct)
            ?? throw new NotFoundException(nameof(Factura), request.Id);

        return new FacturaDto(
    f.FacturaId, f.ClienteId, f.NombreCliente, f.UsuarioId, f.Usuario.Nombre,
    f.TipoPago, f.Total, f.ITBS, f.Pagado, f.Exento, f.Gravado,
    f.Fecha, (int)f.Estado, f.Tarjeta, (int)f.TipoComprobante, (int)f.TipoFactura,
    f.Detalles.Select(d => new FacturaDetalleDto(
        d.FacturaDetalleId, d.ProductoId, d.Producto.Descripcion,
        d.Cantidad, d.SubTotal, d.ITBS, d.Costo, d.Comision)).ToList());
    }
}

public record SearchFacturasQuery(string NombreCliente) : IRequest<List<FacturaListDto>>;

public class SearchFacturasHandler : IRequestHandler<SearchFacturasQuery, List<FacturaListDto>>
{
    private readonly IAppDbContext _context;
    public SearchFacturasHandler(IAppDbContext context) => _context = context;

    public async Task<List<FacturaListDto>> Handle(SearchFacturasQuery request, CancellationToken ct)
        => await _context.Facturas
            .Where(f => f.NombreCliente.StartsWith(request.NombreCliente))
            .OrderByDescending(f => f.Fecha)
            .Take(10)
            .Select(f => new FacturaListDto(
                f.FacturaId, f.NombreCliente, f.Total, f.Pagado,
                f.TipoPago, f.Fecha, (int)f.Estado, (int)f.TipoFactura))
            .ToListAsync(ct);
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearFacturaCommand(
    int? ClienteId,
    int UsuarioId,
    string NombreCliente,
    string TipoPago,
    decimal Total,
    decimal ITBS,
    decimal Pagado,
    decimal Exento,
    decimal Gravado,
    bool Tarjeta,
    int TipoComprobante,
    int TipoFactura,
    List<FacturaDetalleRequest> Detalles
) : IRequest<int>;

public class CrearFacturaValidator : AbstractValidator<CrearFacturaCommand>
{
    public CrearFacturaValidator()
    {
        RuleFor(x => x.NombreCliente).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TipoPago).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Total).GreaterThan(0);
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("La factura debe tener al menos un producto.");
        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(x => x.ProductoId).GreaterThan(0);
            d.RuleFor(x => x.Cantidad).GreaterThan(0);
            d.RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(0);
        });
    }
}

public class CrearFacturaHandler : IRequestHandler<CrearFacturaCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearFacturaHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearFacturaCommand request, CancellationToken ct)
    {
        var factura = new Factura
        {
            ClienteId = request.ClienteId,
            UsuarioId = request.UsuarioId,
            NombreCliente = request.NombreCliente,
            TipoPago = request.TipoPago,
            Total = request.Total,
            ITBS = request.ITBS,
            Pagado = request.Pagado,
            Exento = request.Exento,
            Gravado = request.Gravado,
            Tarjeta = request.Tarjeta,
            TipoComprobante = request.TipoComprobante,
            TipoFactura = (TipoFactura)request.TipoFactura,
            Fecha = DateTime.Now,
            Estado = request.Pagado >= request.Total
                ? EstadoFactura.Saldado
                : request.Pagado > 0
                    ? EstadoFactura.Activo
                    : EstadoFactura.CuentaAbierta
        };

        // Agregar detalles y descontar inventario
        foreach (var d in request.Detalles)
        {
            factura.Detalles.Add(new FacturaDetalle
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                SubTotal = d.SubTotal,
                ITBS = d.ITBS,
                Costo = d.Costo,
                Comision = d.Comision
            });

            var producto = await _context.Productos.FindAsync([d.ProductoId], ct);
            if (producto != null)
                producto.Cantidad -= d.Cantidad;
        }

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync(ct);

        // Registrar comprobante según tipo
        switch (request.TipoComprobante)
        {
            case 0:
                var fiscal = await _context.Fiscals.FirstOrDefaultAsync(ct);
                if (fiscal != null) { fiscal.FacturaId = factura.FacturaId; fiscal.Conteo++; }
                else _context.Fiscals.Add(new Fiscal { FacturaId = factura.FacturaId, Conteo = 1 });
                break;
            case 1:
                var consumo = await _context.Consumos.FirstOrDefaultAsync(ct);
                if (consumo != null) { consumo.FacturaId = factura.FacturaId; consumo.Conteo++; }
                else _context.Consumos.Add(new Consumo { FacturaId = factura.FacturaId, Conteo = 1 });
                break;
            case 2:
                _context.RegimenesEspeciales.Add(new RegimenEspecial { FacturaId = factura.FacturaId, Conteo = 1 });
                break;
            case 3:
                _context.Gubernamentales.Add(new Gubernamental { FacturaId = factura.FacturaId, Conteo = 1 });
                break;
        }

        // Si quedó con deuda, registrar cuenta por cobrar
        if (request.Pagado < request.Total)
        {
            _context.CuentasPorCobrar.Add(new CuentaPorCobrarDetalle
            {
                FacturaId = factura.FacturaId,
                UsuarioId = request.UsuarioId,
                Cantidad = request.Total - request.Pagado,
                FechaUpdate = DateTime.Now
            });
        }

        await _context.SaveChangesAsync(ct);
        return factura.FacturaId;
    }
}

// ── CANCELAR FACTURA ────────────────────
public record CancelarFacturaCommand(int FacturaId) : IRequest;

public class CancelarFacturaHandler : IRequestHandler<CancelarFacturaCommand>
{
    private readonly IAppDbContext _context;
    public CancelarFacturaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(CancelarFacturaCommand request, CancellationToken ct)
    {
        var factura = await _context.Facturas
            .Include(f => f.Detalles)
            .FirstOrDefaultAsync(f => f.FacturaId == request.FacturaId, ct)
            ?? throw new NotFoundException(nameof(Factura), request.FacturaId);

        if (factura.Estado == EstadoFactura.Cancelado)
            throw new BusinessException("La factura ya está cancelada.");

        // Devolver inventario
        foreach (var detalle in factura.Detalles)
        {
            var producto = await _context.Productos.FindAsync([detalle.ProductoId], ct);
            if (producto != null)
                producto.Cantidad += detalle.Cantidad;
        }

        factura.Estado = EstadoFactura.Cancelado;
        await _context.SaveChangesAsync(ct);
    }
}

// ── SALDAR FACTURA (pago parcial o total) ────────────────────
public record SaldarFacturaCommand(int FacturaId, int UsuarioId, decimal Monto) : IRequest;

public class SaldarFacturaValidator : AbstractValidator<SaldarFacturaCommand>
{
    public SaldarFacturaValidator()
    {
        RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a 0.");
    }
}

public class SaldarFacturaHandler : IRequestHandler<SaldarFacturaCommand>
{
    private readonly IAppDbContext _context;
    public SaldarFacturaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(SaldarFacturaCommand request, CancellationToken ct)
    {
        var factura = await _context.Facturas.FindAsync([request.FacturaId], ct)
            ?? throw new NotFoundException(nameof(Factura), request.FacturaId);

        if (factura.Estado == EstadoFactura.Cancelado)
            throw new BusinessException("No se puede saldar una factura cancelada.");

        if (factura.Estado == EstadoFactura.Saldado)
            throw new BusinessException("La factura ya está saldada.");

        factura.Pagado += request.Monto;

        if (factura.Pagado >= factura.Total)
        {
            factura.Pagado = factura.Total;
            factura.Estado = EstadoFactura.Saldado;
        }

        // Registrar abono en cuentas por cobrar
        _context.CuentasPorCobrar.Add(new CuentaPorCobrarDetalle
        {
            FacturaId   = factura.FacturaId,
            UsuarioId   = request.UsuarioId,
            Cantidad    = request.Monto,
            FechaUpdate = DateTime.Now
        });

        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarFacturaCommand(int FacturaId) : IRequest;

public class EliminarFacturaHandler : IRequestHandler<EliminarFacturaCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public EliminarFacturaHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(EliminarFacturaCommand request, CancellationToken ct)
    {
        if (_currentUser.TipoUsuario != 0)  // 0 = Admin
            throw new ForbiddenException();

        var factura = await _context.Facturas.FindAsync([request.FacturaId], ct)
            ?? throw new NotFoundException(nameof(Factura), request.FacturaId);

        _context.Facturas.Remove(factura);
        await _context.SaveChangesAsync(ct);
    }
}

// ── ACTUALIZAR FACTURA (solo estado CuentaAbierta) ────────────────────
public record ActualizarFacturaCommand(
    int FacturaId,
    string TipoPago,
    int TipoFactura,       // 0=Contado, 1=Crédito
    List<FacturaDetalleRequest> Detalles
) : IRequest;

public class ActualizarFacturaValidator : AbstractValidator<ActualizarFacturaCommand>
{
    public ActualizarFacturaValidator()
    {
        RuleFor(x => x.TipoPago).NotEmpty().MaximumLength(15);
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("La factura debe tener al menos un producto.");
        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(x => x.ProductoId).GreaterThan(0);
            d.RuleFor(x => x.Cantidad).GreaterThan(0);
            d.RuleFor(x => x.SubTotal).GreaterThanOrEqualTo(0);
        });
    }
}

public class ActualizarFacturaHandler : IRequestHandler<ActualizarFacturaCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarFacturaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarFacturaCommand request, CancellationToken ct)
    {
        var factura = await _context.Facturas
            .Include(f => f.Detalles)
            .FirstOrDefaultAsync(f => f.FacturaId == request.FacturaId, ct)
            ?? throw new NotFoundException(nameof(Factura), request.FacturaId);

        if (factura.Estado != EstadoFactura.CuentaAbierta)
            throw new BusinessException("Solo se pueden editar facturas en estado Cuenta Abierta.");

        // ── 1. Devolver inventario de los detalles anteriores ──────────
        foreach (var detalle in factura.Detalles)
        {
            var producto = await _context.Productos.FindAsync([detalle.ProductoId], ct);
            if (producto != null)
                producto.Cantidad += detalle.Cantidad;
        }

        // ── 2. Eliminar detalles viejos ────────────────────────────────
        _context.FacturaDetalles.RemoveRange(factura.Detalles);

        // ── 3. Agregar nuevos detalles y descontar inventario ──────────
        var nuevosDetalles = new List<FacturaDetalle>();
        foreach (var d in request.Detalles)
        {
            nuevosDetalles.Add(new FacturaDetalle
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                SubTotal = d.SubTotal,
                ITBS = d.ITBS,
                Costo = d.Costo,
                Comision = d.Comision
            });

            var producto = await _context.Productos.FindAsync([d.ProductoId], ct);
            if (producto != null)
                producto.Cantidad -= d.Cantidad;
        }

        factura.Detalles = nuevosDetalles;

        // ── 4. Recalcular totales ──────────────────────────────────────
        factura.TipoPago = request.TipoPago;
        factura.ITBS = request.Detalles.Sum(d => d.ITBS);
        factura.Gravado = request.Detalles.Where(d => d.ITBS > 0).Sum(d => d.SubTotal - d.ITBS);
        factura.Exento = request.Detalles.Where(d => d.ITBS == 0).Sum(d => d.SubTotal);
        factura.Total = request.Detalles.Sum(d => d.SubTotal);

        // ── 5. Actualizar CuentaPorCobrar con el nuevo pendiente ───────
        var cxc = await _context.CuentasPorCobrar
            .Where(c => c.FacturaId == factura.FacturaId)
            .ToListAsync(ct);

        // El pendiente real es Total - lo que ya pagó
        var nuevoPendiente = factura.Total - factura.Pagado;

        if (cxc.Any())
        {
            // Ajustar el último registro al nuevo saldo
            var ultimo = cxc.OrderByDescending(c => c.FechaUpdate).First();
            ultimo.Cantidad = nuevoPendiente;
            ultimo.FechaUpdate = DateTime.Now;
        }
        else if (nuevoPendiente > 0)
        {
            _context.CuentasPorCobrar.Add(new CuentaPorCobrarDetalle
            {
                FacturaId = factura.FacturaId,
                UsuarioId = factura.UsuarioId,
                Cantidad = nuevoPendiente,
                FechaUpdate = DateTime.Now
            });
        }

        await _context.SaveChangesAsync(ct);
    }
}