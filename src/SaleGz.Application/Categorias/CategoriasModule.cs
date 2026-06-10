using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Categorias;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record CategoriaDto(int CategoriaId, string Descripcion, int Estado, DateTime FechaRegistro);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetCategoriasQuery(int? Estado = null) : IRequest<List<CategoriaDto>>;

public class GetCategoriasHandler : IRequestHandler<GetCategoriasQuery, List<CategoriaDto>>
{
    private readonly IAppDbContext _context;
    public GetCategoriasHandler(IAppDbContext context) => _context = context;

    public async Task<List<CategoriaDto>> Handle(GetCategoriasQuery request, CancellationToken ct)
    {
        var query = _context.Categorias.AsQueryable();

        if (request.Estado.HasValue)
            query = query.Where(c => (int)c.Estado == request.Estado.Value);

        return await query
            .OrderBy(c => c.Descripcion)
            .Select(c => new CategoriaDto(c.CategoriaId, c.Descripcion, (int)c.Estado, c.FechaRegistro))
            .ToListAsync(ct);
    }
}

public record GetCategoriaByIdQuery(int Id) : IRequest<CategoriaDto>;

public class GetCategoriaByIdHandler : IRequestHandler<GetCategoriaByIdQuery, CategoriaDto>
{
    private readonly IAppDbContext _context;
    public GetCategoriaByIdHandler(IAppDbContext context) => _context = context;

    public async Task<CategoriaDto> Handle(GetCategoriaByIdQuery request, CancellationToken ct)
    {
        var c = await _context.Categorias.FindAsync([request.Id], ct)
            ?? throw new NotFoundException(nameof(Categoria), request.Id);

        return new CategoriaDto(c.CategoriaId, c.Descripcion, (int)c.Estado, c.FechaRegistro);
    }
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearCategoriaCommand(int EmpresaId, string Descripcion) : IRequest<int>;

public class CrearCategoriaValidator : AbstractValidator<CrearCategoriaCommand>
{
    public CrearCategoriaValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(100)
            .WithMessage("La descripción es requerida.");
    }
}

public class CrearCategoriaHandler : IRequestHandler<CrearCategoriaCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearCategoriaHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearCategoriaCommand request, CancellationToken ct)
    {
        var categoria = new Categoria
        {
            EmpresaId     = request.EmpresaId,
            Descripcion   = request.Descripcion,
            Estado        = EstadoGeneral.Activo,
            FechaRegistro = DateTime.Now
        };

        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync(ct);
        return categoria.CategoriaId;
    }
}

public record ActualizarCategoriaCommand(int CategoriaId, string Descripcion) : IRequest;

public class ActualizarCategoriaValidator : AbstractValidator<ActualizarCategoriaCommand>
{
    public ActualizarCategoriaValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(100);
    }
}

public class ActualizarCategoriaHandler : IRequestHandler<ActualizarCategoriaCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarCategoriaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await _context.Categorias.FindAsync([request.CategoriaId], ct)
            ?? throw new NotFoundException(nameof(Categoria), request.CategoriaId);

        categoria.Descripcion = request.Descripcion;
        await _context.SaveChangesAsync(ct);
    }
}

public record ToggleEstadoCategoriaCommand(int CategoriaId) : IRequest;

public class ToggleEstadoCategoriaHandler : IRequestHandler<ToggleEstadoCategoriaCommand>
{
    private readonly IAppDbContext _context;
    public ToggleEstadoCategoriaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ToggleEstadoCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await _context.Categorias.FindAsync([request.CategoriaId], ct)
            ?? throw new NotFoundException(nameof(Categoria), request.CategoriaId);

        categoria.Estado = categoria.Estado == EstadoGeneral.Activo
            ? EstadoGeneral.Inactivo
            : EstadoGeneral.Activo;

        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarCategoriaCommand(int CategoriaId) : IRequest;

public class EliminarCategoriaHandler : IRequestHandler<EliminarCategoriaCommand>
{
    private readonly IAppDbContext _context;
    public EliminarCategoriaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(EliminarCategoriaCommand request, CancellationToken ct)
    {
        var categoria = await _context.Categorias.FindAsync([request.CategoriaId], ct)
            ?? throw new NotFoundException(nameof(Categoria), request.CategoriaId);

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync(ct);
    }
}
