using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;

namespace SaleGz.Application.Gastos;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record ConsumoGastoDto(int ConsumosGastosId, string Descripcion);
public record GastoDetalleDto(int GastosDetalleId, int ConsumosGastosId, string Descripcion, decimal Monto);
public record GastoDto(int GastosId, int UsuarioId, string Usuario, DateTime Fecha, decimal Total, List<GastoDetalleDto> Detalles);
public record GastoListDto(int GastosId, string Usuario, DateTime Fecha, decimal Total, List<GastoDetalleDto> Detalles);
public record GastoDetalleRequest(int ConsumosGastosId, decimal Monto);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetGastosQuery(DateTime? Desde = null, DateTime? Hasta = null) : IRequest<List<GastoListDto>>;

public class GetGastosHandler : IRequestHandler<GetGastosQuery, List<GastoListDto>>
{
    private readonly IAppDbContext _context;
    public GetGastosHandler(IAppDbContext context) => _context = context;

    public async Task<List<GastoListDto>> Handle(GetGastosQuery request, CancellationToken ct)
    {
        var query = _context.Gastos
            .Include(g => g.Usuario)
            .Include(g => g.Detalles).ThenInclude(d => d.ConsumoGasto)
            .AsQueryable();

        if (request.Desde.HasValue) query = query.Where(g => g.Fecha >= request.Desde.Value);
        if (request.Hasta.HasValue) query = query.Where(g => g.Fecha <= request.Hasta.Value);

        return await query
            .OrderByDescending(g => g.Fecha)
            .Select(g => new GastoListDto(
                g.GastosId,
                g.Usuario.Nombre,
                g.Fecha,
                g.Total,
                g.Detalles.Select(d => new GastoDetalleDto(
                    d.GastosDetalleId,
                    d.ConsumosGastosId,
                    d.ConsumoGasto.Descripcion,
                    d.Monto)).ToList()))
            .ToListAsync(ct);
    }
}

public record GetGastoByIdQuery(int Id) : IRequest<GastoDto>;

public class GetGastoByIdHandler : IRequestHandler<GetGastoByIdQuery, GastoDto>
{
    private readonly IAppDbContext _context;
    public GetGastoByIdHandler(IAppDbContext context) => _context = context;

    public async Task<GastoDto> Handle(GetGastoByIdQuery request, CancellationToken ct)
    {
        var g = await _context.Gastos
            .Include(x => x.Detalles).ThenInclude(d => d.ConsumoGasto)
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.GastosId == request.Id, ct)
            ?? throw new NotFoundException(nameof(Gasto), request.Id);

        return new GastoDto(g.GastosId, g.UsuarioId, g.Usuario.Nombre, g.Fecha, g.Total,
            g.Detalles.Select(d => new GastoDetalleDto(
                d.GastosDetalleId, d.ConsumosGastosId, d.ConsumoGasto.Descripcion, d.Monto)).ToList());
    }
}

public record GetConsumosGastosQuery : IRequest<List<ConsumoGastoDto>>;

public class GetConsumosGastosHandler : IRequestHandler<GetConsumosGastosQuery, List<ConsumoGastoDto>>
{
    private readonly IAppDbContext _context;
    public GetConsumosGastosHandler(IAppDbContext context) => _context = context;

    public async Task<List<ConsumoGastoDto>> Handle(GetConsumosGastosQuery request, CancellationToken ct)
        => await _context.ConsumosGastos
            .OrderBy(c => c.Descripcion)
            .Select(c => new ConsumoGastoDto(c.ConsumosGastosId, c.Descripcion))
            .ToListAsync(ct);
}

public record SearchConsumosGastosQuery(string Descripcion) : IRequest<List<ConsumoGastoDto>>;

public class SearchConsumosGastosHandler : IRequestHandler<SearchConsumosGastosQuery, List<ConsumoGastoDto>>
{
    private readonly IAppDbContext _context;
    public SearchConsumosGastosHandler(IAppDbContext context) => _context = context;

    public async Task<List<ConsumoGastoDto>> Handle(SearchConsumosGastosQuery request, CancellationToken ct)
        => await _context.ConsumosGastos
            .Where(c => c.Descripcion.StartsWith(request.Descripcion))
            .Take(10)
            .Select(c => new ConsumoGastoDto(c.ConsumosGastosId, c.Descripcion))
            .ToListAsync(ct);
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearGastoCommand(int UsuarioId, List<GastoDetalleRequest> Detalles) : IRequest<int>;

public class CrearGastoValidator : AbstractValidator<CrearGastoCommand>
{
    public CrearGastoValidator()
    {
        RuleFor(x => x.Detalles).NotEmpty().WithMessage("El gasto debe tener al menos un concepto.");
        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(x => x.Monto).GreaterThan(0);
            d.RuleFor(x => x.ConsumosGastosId).GreaterThan(0);
        });
    }
}

public class CrearGastoHandler : IRequestHandler<CrearGastoCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearGastoHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearGastoCommand request, CancellationToken ct)
    {
        var total = request.Detalles.Sum(d => d.Monto);

        var gasto = new Gasto
        {
            UsuarioId = request.UsuarioId,
            Total     = total,
            Fecha     = DateTime.Now
        };

        foreach (var d in request.Detalles)
        {
            gasto.Detalles.Add(new GastoDetalle
            {
                ConsumosGastosId = d.ConsumosGastosId,
                Monto            = d.Monto
            });
        }

        _context.Gastos.Add(gasto);
        await _context.SaveChangesAsync(ct);
        return gasto.GastosId;
    }
}

public record CrearConsumoGastoCommand(string Descripcion) : IRequest<int>;

public class CrearConsumoGastoValidator : AbstractValidator<CrearConsumoGastoCommand>
{
    public CrearConsumoGastoValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public class CrearConsumoGastoHandler : IRequestHandler<CrearConsumoGastoCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearConsumoGastoHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearConsumoGastoCommand request, CancellationToken ct)
    {
        var consumo = new ConsumoGasto { Descripcion = request.Descripcion };
        _context.ConsumosGastos.Add(consumo);
        await _context.SaveChangesAsync(ct);
        return consumo.ConsumosGastosId;
    }
}

public record EliminarGastoCommand(int GastosId) : IRequest;

public class EliminarGastoHandler : IRequestHandler<EliminarGastoCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public EliminarGastoHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(EliminarGastoCommand request, CancellationToken ct)
    {
        if (_currentUser.TipoUsuario != 0)  // 0 = Admin
            throw new ForbiddenException();

        var gasto = await _context.Gastos.FindAsync([request.GastosId], ct)
            ?? throw new NotFoundException(nameof(Gasto), request.GastosId);

        _context.Gastos.Remove(gasto);
        await _context.SaveChangesAsync(ct);
    }
}

public record ActualizarGastoCommand(
    int GastosId,
    List<GastoDetalleRequest> Detalles
) : IRequest;

public class ActualizarGastoValidator : AbstractValidator<ActualizarGastoCommand>
{
    public ActualizarGastoValidator()
    {
        RuleFor(x => x.GastosId).GreaterThan(0);
        RuleFor(x => x.Detalles).NotEmpty();

        RuleForEach(x => x.Detalles).ChildRules(d =>
        {
            d.RuleFor(x => x.Monto).GreaterThan(0);
            d.RuleFor(x => x.ConsumosGastosId).GreaterThan(0);
        });
    }
}

public class ActualizarGastoHandler : IRequestHandler<ActualizarGastoCommand>
{
    private readonly IAppDbContext _context;

    public ActualizarGastoHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ActualizarGastoCommand request, CancellationToken ct)
    {
        var gasto = await _context.Gastos
            .Include(g => g.Detalles)
            .FirstOrDefaultAsync(g => g.GastosId == request.GastosId, ct)
            ?? throw new NotFoundException(nameof(Gasto), request.GastosId);

        // 🔥 recalcular total
        var total = request.Detalles.Sum(d => d.Monto);
        gasto.Total = total;

        // 🔥 eliminar detalles actuales
        _context.GastoDetalles.RemoveRange(gasto.Detalles);

        // 🔥 crear nuevos detalles
        gasto.Detalles = request.Detalles.Select(d => new GastoDetalle
        {
            ConsumosGastosId = d.ConsumosGastosId,
            Monto = d.Monto
        }).ToList();

        await _context.SaveChangesAsync(ct);
    }
}

// Command
public record ActualizarConsumoGastoCommand(int ConsumosGastosId, string Descripcion) : IRequest;

public class ActualizarConsumoGastoValidator : AbstractValidator<ActualizarConsumoGastoCommand>
{
    public ActualizarConsumoGastoValidator()
    {
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
    }
}

public class ActualizarConsumoGastoHandler : IRequestHandler<ActualizarConsumoGastoCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarConsumoGastoHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarConsumoGastoCommand request, CancellationToken ct)
    {
        var consumo = await _context.ConsumosGastos.FindAsync([request.ConsumosGastosId], ct)
            ?? throw new NotFoundException(nameof(ConsumoGasto), request.ConsumosGastosId);

        consumo.Descripcion = request.Descripcion;
        await _context.SaveChangesAsync(ct);
    }
}
