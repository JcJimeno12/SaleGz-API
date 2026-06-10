using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;

namespace SaleGz.Application.Inventario;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record MovimientoInventarioDto(
    int MovimientoId,
    int ProductoId,
    string ProductoDescripcion,
    int Tipo,
    string TipoNombre,
    decimal Cantidad,
    string? Referencia,
    int UsuarioId,
    string UsuarioNombre,
    DateTime Fecha,
    string? Nota,
    decimal StockAnterior,   
    decimal StockActual
);

public record MovimientoInventarioDetalleDto(
    int ProductoId,
    string ProductoDescripcion,
    float CantidadAnterior,
    float CantidadMovimiento,
    float CantidadActual
);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetMovimientosInventarioQuery(
    int? ProductoId = null,
    int? Tipo = null,
    DateTime? FechaInicio = null,
    DateTime? FechaFin = null
) : IRequest<List<MovimientoInventarioDto>>;



public class GetMovimientosInventarioHandler : IRequestHandler<GetMovimientosInventarioQuery, List<MovimientoInventarioDto>>
{
  
    private readonly IAppDbContext _context;
    public GetMovimientosInventarioHandler(IAppDbContext context) => _context = context;

    public async Task<List<MovimientoInventarioDto>> Handle(GetMovimientosInventarioQuery request, CancellationToken ct)
    {
        var query = _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .AsQueryable();

        if (request.ProductoId.HasValue)
            query = query.Where(m => m.ProductoId == request.ProductoId.Value);

        if (request.Tipo.HasValue)
            query = query.Where(m => m.Tipo == request.Tipo.Value);

        if (request.FechaInicio.HasValue)
            query = query.Where(m => m.Fecha >= request.FechaInicio.Value);

        if (request.FechaFin.HasValue)
            query = query.Where(m => m.Fecha <= request.FechaFin.Value.AddDays(1));

        return await query
     .OrderByDescending(m => m.Fecha)
     .Select(m => new MovimientoInventarioDto(
         m.MovimientoId,
         m.ProductoId,
         m.Producto.Descripcion,
         m.Tipo,
         GetTipoNombre(m.Tipo),
         m.Cantidad,
         m.Referencia,
         m.UsuarioId,
         m.Usuario.Nombre,
         m.Fecha,
         m.Nota,
         m.StockAnterior,    
         m.StockActual       
     ))
     .ToListAsync(ct);
    }

    private static string GetTipoNombre(int tipo) => tipo switch
    {
        0 => "Entrada",
        1 => "Salida",
        2 => "Ajuste",
        _ => "Desconocido"
    };
}

public record GetMovimientoInventarioByIdQuery(int Id) : IRequest<MovimientoInventarioDto>;

public class GetMovimientoInventarioByIdHandler : IRequestHandler<GetMovimientoInventarioByIdQuery, MovimientoInventarioDto>
{
    private static MovimientoInventarioDto Map(MovimientoInventario m)
    {
        return new MovimientoInventarioDto(
            m.MovimientoId,
            m.ProductoId,
            m.Producto.Descripcion,
            m.Tipo,
            GetTipoNombre(m.Tipo),
            m.Cantidad,
            m.Referencia,
            m.UsuarioId,
            m.Usuario.Nombre,
            m.Fecha,
            m.Nota,
            m.StockAnterior,
            m.StockActual
        );
    }


    private readonly IAppDbContext _context;
    public GetMovimientoInventarioByIdHandler(IAppDbContext context) => _context = context;

    public async Task<MovimientoInventarioDto> Handle(GetMovimientoInventarioByIdQuery request, CancellationToken ct)
    {
        var m = await _context.MovimientosInventario
            .Include(x => x.Producto)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.MovimientoId == request.Id, ct)
            ?? throw new NotFoundException(nameof(MovimientoInventario), request.Id);

        return new MovimientoInventarioDto(
                m.MovimientoId,
                m.ProductoId,
                m.Producto.Descripcion,
                m.Tipo,
                GetTipoNombre(m.Tipo),
                m.Cantidad,
                m.Referencia,
                m.UsuarioId,
                m.Usuario.Nombre,
                m.Fecha,
                m.Nota,
                m.StockAnterior,   
                m.StockActual      
            );
    }

    private static string GetTipoNombre(int tipo) => tipo switch
    {
        0 => "Entrada",
        1 => "Salida",
        2 => "Ajuste",
        _ => "Desconocido"
    };
}

// ── HISTORIAL POR PRODUCTO ──────────────
public record GetMovimientosProductoQuery(int ProductoId, int? Limit = 50) : IRequest<List<MovimientoInventarioDto>>;

public class GetMovimientosProductoHandler : IRequestHandler<GetMovimientosProductoQuery, List<MovimientoInventarioDto>>
{
    private readonly IAppDbContext _context;
    public GetMovimientosProductoHandler(IAppDbContext context) => _context = context;

    public async Task<List<MovimientoInventarioDto>> Handle(GetMovimientosProductoQuery request, CancellationToken ct)
    {
        return await _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .Where(m => m.ProductoId == request.ProductoId)
            .OrderByDescending(m => m.Fecha)
            .Take(request.Limit ?? 50)
            .Select(m => MovimientoInventarioMapper.Map(m))
            .ToListAsync(ct);
    }

    private static string GetTipoNombre(int tipo) => tipo switch
    {
        0 => "Entrada",
        1 => "Salida",
        2 => "Ajuste",
        _ => "Desconocido"
    };
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearMovimientoInventarioCommand(
    int ProductoId,
    int Tipo,  // 0=Entrada, 1=Salida, 2=Ajuste
    decimal Cantidad,
    string? Referencia, 
    string? Nota
) : IRequest<int>;

public class CrearMovimientoInventarioValidator : AbstractValidator<CrearMovimientoInventarioCommand>
{
    public CrearMovimientoInventarioValidator()
    {
        RuleFor(x => x.ProductoId).GreaterThan(0).WithMessage("Debe seleccionar un producto.");
        RuleFor(x => x.Tipo).InclusiveBetween(0, 2).WithMessage("Tipo de movimiento inválido.");
        RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
        //RuleFor(x => x.UsuarioId).GreaterThan(0).WithMessage("Usuario inválido.");
    }
}

public class CrearMovimientoInventarioHandler : IRequestHandler<CrearMovimientoInventarioCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CrearMovimientoInventarioHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CrearMovimientoInventarioCommand request, CancellationToken ct)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync(ct);

            var producto = await _context.Productos
                .FirstOrDefaultAsync(x => x.ProductoId == request.ProductoId, ct)
                ?? throw new NotFoundException(nameof(Producto), request.ProductoId);

            var stockAnterior = producto.Cantidad;
            decimal stockNuevo = stockAnterior;

            // 🔥 LÓGICA INVENTARIO
            if (request.Tipo == 0) // Entrada
            {
                stockNuevo += request.Cantidad;
            }
            else if (request.Tipo == 1) // Salida
            {
                if (stockAnterior < request.Cantidad)
                    throw new Exception("Stock insuficiente");

                stockNuevo -= request.Cantidad;
            }
            else if (request.Tipo == 2) // Ajuste
            {
                stockNuevo = request.Cantidad;
            }

            producto.Cantidad = stockNuevo;

            var movimiento = new MovimientoInventario
            {
                ProductoId = request.ProductoId,
                Tipo = request.Tipo,
                Cantidad = request.Cantidad,
                StockAnterior = stockAnterior,
                StockActual = stockNuevo,
                UsuarioId = _currentUser.UsuarioId,
                Fecha = DateTime.UtcNow,
                Referencia = request.Referencia,
                Nota = request.Nota
            };

            _context.MovimientosInventario.Add(movimiento);

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return movimiento.MovimientoId;
        });
    }
}