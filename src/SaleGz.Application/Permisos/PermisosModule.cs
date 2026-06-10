 

using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;

namespace SaleGz.Application.Usuarios;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════
public record PermisoDto(
    int  UsuarioId,
    bool VerFacturas,       bool CrearFacturas,     bool CancelarFacturas,
    bool VerCotizaciones,   bool CrearCotizaciones,
    bool VerCompras,        bool CrearCompras,
    bool VerGastos,         bool CrearGastos,
    bool VerCuadres,        bool CrearCuadres,
    bool VerClientes,       bool CrearClientes,
    bool VerProductos,
    bool VerInventario,      bool CrearInventario,
    bool VerReportes
);

// ════════════════════════════════════════
// QUERY
// ════════════════════════════════════════
public record GetPermisosQuery(int UsuarioId) : IRequest<PermisoDto>;

public class GetPermisosHandler : IRequestHandler<GetPermisosQuery, PermisoDto>
{
    private readonly IAppDbContext _context;
    public GetPermisosHandler(IAppDbContext context) => _context = context;

    public async Task<PermisoDto> Handle(GetPermisosQuery request, CancellationToken ct)
    {
        var p = await _context.UsuarioPermisos
            .FirstOrDefaultAsync(x => x.UsuarioId == request.UsuarioId, ct);

        // Si no existe, devolver permisos por defecto
        if (p == null) return DefaultPermisos(request.UsuarioId);

        return MapToDto(p);
    }

    public static PermisoDto DefaultPermisos(int usuarioId) => new(
        usuarioId,
        true,  true,  false,   // Facturas
        true,  true,           // Cotizaciones
        false, false,          // Compras
        false, false,          // Gastos
        true,  true,           // Cuadres
        true,  true,           // Clientes
        true,  false, false,     // Inventario (VerInventario, CrearInventario) - desactivado por defecto
        false                  // Reportes
    );

    private static PermisoDto MapToDto(UsuarioPermiso p) => new(
        p.UsuarioId,
        p.VerFacturas,       p.CrearFacturas,     p.CancelarFacturas,
        p.VerCotizaciones,   p.CrearCotizaciones,
        p.VerCompras,        p.CrearCompras,
        p.VerGastos,         p.CrearGastos,
        p.VerCuadres,        p.CrearCuadres,
        p.VerClientes,       p.CrearClientes,
        p.VerProductos,
        p.VerInventario,      p.CrearInventario,
        p.VerReportes
    );
}

// ════════════════════════════════════════
// COMMAND
// ════════════════════════════════════════
public record GuardarPermisosCommand(
    int  UsuarioId,
    bool VerFacturas,       bool CrearFacturas,     bool CancelarFacturas,
    bool VerCotizaciones,   bool CrearCotizaciones,
    bool VerCompras,        bool CrearCompras,
    bool VerGastos,         bool CrearGastos,
    bool VerCuadres,        bool CrearCuadres,
    bool VerClientes,       bool CrearClientes,
    bool VerProductos,
    bool VerInventario,      bool CrearInventario,
    bool VerReportes
) : IRequest;

public class GuardarPermisosHandler : IRequestHandler<GuardarPermisosCommand>
{
    private readonly IAppDbContext _context;
    public GuardarPermisosHandler(IAppDbContext context) => _context = context;

    public async Task Handle(GuardarPermisosCommand r, CancellationToken ct)
    {
        var p = await _context.UsuarioPermisos
            .FirstOrDefaultAsync(x => x.UsuarioId == r.UsuarioId, ct);

        if (p == null)
        {
            p = new UsuarioPermiso { UsuarioId = r.UsuarioId };
            _context.UsuarioPermisos.Add(p);
        }

        p.VerFacturas       = r.VerFacturas;
        p.CrearFacturas     = r.CrearFacturas;
        p.CancelarFacturas  = r.CancelarFacturas;
        p.VerCotizaciones   = r.VerCotizaciones;
        p.CrearCotizaciones = r.CrearCotizaciones;
        p.VerCompras        = r.VerCompras;
        p.CrearCompras      = r.CrearCompras;
        p.VerGastos         = r.VerGastos;
        p.CrearGastos       = r.CrearGastos;
        p.VerCuadres        = r.VerCuadres;
        p.CrearCuadres      = r.CrearCuadres;
        p.VerClientes       = r.VerClientes;
        p.CrearClientes     = r.CrearClientes;
        p.VerProductos      = r.VerProductos;
        p.VerInventario     = r.VerInventario;
        p.CrearInventario   = r.CrearInventario;
        p.VerReportes       = r.VerReportes;

        await _context.SaveChangesAsync(ct);
    }
}
