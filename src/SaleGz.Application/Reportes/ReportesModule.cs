using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Reportes;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record ReporteVentasDto(
    decimal TotalVentas,
    decimal TotalITBS,
    decimal TotalCobrado,
    decimal TotalPendiente,
    int CantidadFacturas,
    int FacturasCanceladas
);

public record ReporteCuentasCobrarDto(
    int FacturaId,
    string NombreCliente,
    decimal Total,
    decimal Pagado,
    decimal Pendiente,
    DateTime Fecha
);

public record ReporteInventarioDto(
    int ProductoId,
    string Descripcion,
    string Categoria,
    decimal Cantidad,
    int Minimo,
    decimal Costo,
    decimal Precio1,
    bool StockBajo
);

public record ReporteInventarioResumenDto(
    int TotalProductos,
    decimal ValorTotalCosto,
    decimal ValorTotalPrecio1,
    int ProductosStockBajo
);


public record ReporteGastosDto(decimal TotalGastos, int CantidadGastos, DateTime Desde, DateTime Hasta);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetReporteVentasQuery(DateTime Desde, DateTime Hasta) : IRequest<ReporteVentasDto>;

public class GetReporteVentasHandler : IRequestHandler<GetReporteVentasQuery, ReporteVentasDto>
{
    private readonly IAppDbContext _context;
    public GetReporteVentasHandler(IAppDbContext context) => _context = context;

    public async Task<ReporteVentasDto> Handle(GetReporteVentasQuery request, CancellationToken ct)
    {
        // Ajustar hasta al final del día
        var hastaFin = request.Hasta.Date.AddDays(1).AddTicks(-1);

        var facturas = await _context.Facturas
            .Where(f => f.Fecha >= request.Desde.Date && f.Fecha <= hastaFin)
            .ToListAsync(ct);

        var activas = facturas.Where(f => f.Estado != EstadoFactura.Cancelado).ToList();

        return new ReporteVentasDto(
            TotalVentas: activas.Sum(f => f.Total),
            TotalITBS: activas.Sum(f => f.ITBS),
            TotalCobrado: activas.Sum(f => f.Pagado),
            TotalPendiente: activas.Sum(f => f.Total - f.Pagado),
            CantidadFacturas: activas.Count,
            FacturasCanceladas: facturas.Count(f => f.Estado == EstadoFactura.Cancelado)
        );
    }
}

public record GetReporteCuentasCobrarQuery : IRequest<List<ReporteCuentasCobrarDto>>;

public class GetReporteCuentasCobrarHandler : IRequestHandler<GetReporteCuentasCobrarQuery, List<ReporteCuentasCobrarDto>>
{
    private readonly IAppDbContext _context;
    public GetReporteCuentasCobrarHandler(IAppDbContext context) => _context = context;

    public async Task<List<ReporteCuentasCobrarDto>> Handle(GetReporteCuentasCobrarQuery request, CancellationToken ct)
        => await _context.Facturas
            .Where(f => f.Estado == EstadoFactura.CuentaAbierta || f.Estado == EstadoFactura.Activo)
            .Where(f => f.Pagado < f.Total)
            .OrderByDescending(f => f.Fecha)
            .Select(f => new ReporteCuentasCobrarDto(
                f.FacturaId, f.NombreCliente, f.Total, f.Pagado,
                f.Total - f.Pagado, f.Fecha))
            .ToListAsync(ct);
}

public record GetReporteInventarioQuery : IRequest<List<ReporteInventarioDto>>;

public class GetReporteInventarioHandler : IRequestHandler<GetReporteInventarioQuery, List<ReporteInventarioDto>>
{
    private readonly IAppDbContext _context;
    public GetReporteInventarioHandler(IAppDbContext context) => _context = context;

    public async Task<List<ReporteInventarioDto>> Handle(GetReporteInventarioQuery request, CancellationToken ct)
        => await _context.Productos
            .Include(p => p.Categoria)
            .OrderBy(p => p.Descripcion)
            .Select(p => new ReporteInventarioDto(
                p.ProductoId, p.Descripcion, p.Categoria.Descripcion,
                p.Cantidad, p.Minimo, p.Costo, p.Precio1,
                p.Cantidad <= p.Minimo))
            .ToListAsync(ct);
}

public record GetReporteGastosQuery(DateTime Desde, DateTime Hasta) : IRequest<ReporteGastosDto>;

public class GetReporteGastosHandler : IRequestHandler<GetReporteGastosQuery, ReporteGastosDto>
{
    private readonly IAppDbContext _context;
    public GetReporteGastosHandler(IAppDbContext context) => _context = context;

    public async Task<ReporteGastosDto> Handle(GetReporteGastosQuery request, CancellationToken ct)
    {
        var gastos = await _context.Gastos
            .Where(g => g.Fecha >= request.Desde && g.Fecha <= request.Hasta)
            .ToListAsync(ct);

        return new ReporteGastosDto(
            TotalGastos:    gastos.Sum(g => g.Total),
            CantidadGastos: gastos.Count,
            Desde:          request.Desde,
            Hasta:          request.Hasta);
    }
}


public record GetResumenInventarioQuery : IRequest<ReporteInventarioResumenDto>;

public class GetResumenInventarioHandler : IRequestHandler<GetResumenInventarioQuery, ReporteInventarioResumenDto>
{
    private readonly IAppDbContext _context;
    public GetResumenInventarioHandler(IAppDbContext context) => _context = context;

    public async Task<ReporteInventarioResumenDto> Handle(GetResumenInventarioQuery request, CancellationToken ct)
    {
        var productos = await _context.Productos.ToListAsync(ct);

        return new ReporteInventarioResumenDto(
            TotalProductos: productos.Count,
            ValorTotalCosto: productos.Sum(p => p.Cantidad * p.Costo),
            ValorTotalPrecio1: productos.Sum(p => p.Cantidad * p.Precio1),
            ProductosStockBajo: productos.Count(p => p.Cantidad <= p.Minimo)
        );
    }
}