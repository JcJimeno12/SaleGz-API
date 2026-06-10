using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Productos;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record ProductoDto(
    int ProductoId,
    int CategoriaId,
    string Categoria,
    string Descripcion,
    string? Referencia,
    decimal Cantidad,
    decimal Costo,
    int Minimo,
    decimal Precio1,
    decimal Precio2,
    decimal Precio3,
    int ITBIS,
    string? CodigoBarra
);

public record ProductoSearchDto(
    int ProductoId,
    string Descripcion,
    decimal Cantidad,
    decimal Precio1,
    decimal Precio2,
    decimal Precio3
);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetProductosQuery(int? CategoriaId = null) : IRequest<List<ProductoDto>>;

public class GetProductosHandler : IRequestHandler<GetProductosQuery, List<ProductoDto>>
{
    private readonly IAppDbContext _context;
    public GetProductosHandler(IAppDbContext context) => _context = context;

    public async Task<List<ProductoDto>> Handle(GetProductosQuery request, CancellationToken ct)
    {
        var query = _context.Productos
            .Include(p => p.Categoria)
            .AsQueryable();

        if (request.CategoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == request.CategoriaId.Value);

        return await query
            .OrderBy(p => p.Descripcion)
            .Select(p => new ProductoDto(
                p.ProductoId, p.CategoriaId, p.Categoria.Descripcion,
                p.Descripcion, p.Referencia, p.Cantidad, p.Costo, p.Minimo,
                p.Precio1, p.Precio2, p.Precio3, (int)p.ITBIS, p.CodigoBarra))
            .ToListAsync(ct);
    }
}

public record GetProductoByIdQuery(int Id) : IRequest<ProductoDto>;

public class GetProductoByIdHandler : IRequestHandler<GetProductoByIdQuery, ProductoDto>
{
    private readonly IAppDbContext _context;
    public GetProductoByIdHandler(IAppDbContext context) => _context = context;

    public async Task<ProductoDto> Handle(GetProductoByIdQuery request, CancellationToken ct)
    {
        var p = await _context.Productos
            .Include(x => x.Categoria)
            .FirstOrDefaultAsync(x => x.ProductoId == request.Id, ct)
            ?? throw new NotFoundException(nameof(Producto), request.Id);

        return new ProductoDto(p.ProductoId, p.CategoriaId, p.Categoria.Descripcion,
            p.Descripcion, p.Referencia, p.Cantidad, p.Costo, p.Minimo,
            p.Precio1, p.Precio2, p.Precio3, (int)p.ITBIS, p.CodigoBarra);
    }
}

// ── SEARCH (autocomplete) ───────────────
public record SearchProductosQuery(string Descripcion) : IRequest<List<ProductoSearchDto>>;

public class SearchProductosHandler : IRequestHandler<SearchProductosQuery, List<ProductoSearchDto>>
{
    private readonly IAppDbContext _context;
    public SearchProductosHandler(IAppDbContext context) => _context = context;

    public async Task<List<ProductoSearchDto>> Handle(SearchProductosQuery request, CancellationToken ct)
    {
        return await _context.Productos
            .Where(p => p.Descripcion.StartsWith(request.Descripcion))
            .OrderBy(p => p.Descripcion)
            .Take(10)
            .Select(p => new ProductoSearchDto(
                p.ProductoId, p.Descripcion, p.Cantidad,
                p.Precio1, p.Precio2, p.Precio3))
            .ToListAsync(ct);
    }
}

// ── STOCK BAJO ──────────────────────────
public record GetProductosStockBajoQuery : IRequest<List<ProductoDto>>;

public class GetProductosStockBajoHandler : IRequestHandler<GetProductosStockBajoQuery, List<ProductoDto>>
{
    private readonly IAppDbContext _context;
    public GetProductosStockBajoHandler(IAppDbContext context) => _context = context;

    public async Task<List<ProductoDto>> Handle(GetProductosStockBajoQuery request, CancellationToken ct)
    {
        return await _context.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Cantidad <= p.Minimo)
            .OrderBy(p => p.Cantidad)
            .Select(p => new ProductoDto(
                p.ProductoId, p.CategoriaId, p.Categoria.Descripcion,
                p.Descripcion, p.Referencia, p.Cantidad, p.Costo, p.Minimo,
                p.Precio1, p.Precio2, p.Precio3, (int)p.ITBIS, p.CodigoBarra))
            .ToListAsync(ct);
    }
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearProductoCommand(
    int EmpresaId,
    int CategoriaId,
    string Descripcion,
    string? Referencia,
    decimal Cantidad,
    decimal Costo,
    int Minimo,
    decimal Precio1,
    decimal Precio2,
    decimal Precio3,
    int ITBIS,
    string? CodigoBarra
) : IRequest<int>;

public class CrearProductoValidator : AbstractValidator<CrearProductoCommand>
{
    public CrearProductoValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CategoriaId).GreaterThan(0).WithMessage("Debe seleccionar una categoría.");
        RuleFor(x => x.Precio1).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Costo).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cantidad).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ITBIS).InclusiveBetween(0, 1).WithMessage("ITBIS debe ser 0 (Exento) o 1 (Gravado).");
    }
}

public class CrearProductoHandler : IRequestHandler<CrearProductoCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearProductoHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearProductoCommand request, CancellationToken ct)
    {
        var producto = new Producto
        {
            EmpresaId   = request.EmpresaId,
            CategoriaId = request.CategoriaId,
            Descripcion = request.Descripcion,
            Referencia  = request.Referencia,
            Cantidad    = request.Cantidad,
            Costo       = request.Costo,
            Minimo      = request.Minimo,
            Precio1     = request.Precio1,
            Precio2     = request.Precio2,
            Precio3     = request.Precio3,
            ITBIS       = (TipoITBIS)request.ITBIS,
            CodigoBarra = request.CodigoBarra
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync(ct);
        return producto.ProductoId;
    }
}

public record ActualizarProductoCommand(
    int ProductoId,
    int CategoriaId,
    string Descripcion,
    string? Referencia,
    decimal Cantidad,
    decimal Costo,
    int Minimo,
    decimal Precio1,
    decimal Precio2,
    decimal Precio3,
    int ITBIS,
    string? CodigoBarra
) : IRequest;

public class ActualizarProductoValidator : AbstractValidator<ActualizarProductoCommand>
{
    public ActualizarProductoValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CategoriaId).GreaterThan(0);
        RuleFor(x => x.Precio1).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Costo).GreaterThanOrEqualTo(0);
    }
}

public class ActualizarProductoHandler : IRequestHandler<ActualizarProductoCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarProductoHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarProductoCommand request, CancellationToken ct)
    {
        var producto = await _context.Productos.FindAsync([request.ProductoId], ct)
            ?? throw new NotFoundException(nameof(Producto), request.ProductoId);

        producto.CategoriaId = request.CategoriaId;
        producto.Descripcion = request.Descripcion;
        producto.Referencia  = request.Referencia;
        producto.Cantidad    = request.Cantidad;
        producto.Costo       = request.Costo;
        producto.Minimo      = request.Minimo;
        producto.Precio1     = request.Precio1;
        producto.Precio2     = request.Precio2;
        producto.Precio3     = request.Precio3;
        producto.ITBIS       = (TipoITBIS)request.ITBIS;
        producto.CodigoBarra = request.CodigoBarra;

        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarProductoCommand(int ProductoId) : IRequest;

public class EliminarProductoHandler : IRequestHandler<EliminarProductoCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public EliminarProductoHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(EliminarProductoCommand request, CancellationToken ct)
    {
        if (_currentUser.TipoUsuario != 0)  // 0 = Admin
            throw new ForbiddenException();

        var producto = await _context.Productos.FindAsync([request.ProductoId], ct)
            ?? throw new NotFoundException(nameof(Producto), request.ProductoId);

        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync(ct);
    }
}
