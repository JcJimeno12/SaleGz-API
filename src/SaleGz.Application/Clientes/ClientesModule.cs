using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Clientes;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record ClienteDto(
    int ClienteId,
    string Nombre,
    string? Telefono,
    string? Email,
    string? Cedula,
    string? Direccion,
    decimal Credito,
    int Estado,
    bool Sexo,
    DateTime? FechaNacimiento,
    DateTime FechaRegistro,
    int Tipo,
    int TipoEntidad
);

public record ClienteSearchDto(int ClienteId, string Nombre, string? Telefono, string? Cedula);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

// ── GET ALL ─────────────────────────────
public record GetClientesQuery(int? Estado = null) : IRequest<List<ClienteDto>>;

public class GetClientesHandler : IRequestHandler<GetClientesQuery, List<ClienteDto>>
{
    private readonly IAppDbContext _context;
    public GetClientesHandler(IAppDbContext context) => _context = context;

    public async Task<List<ClienteDto>> Handle(GetClientesQuery request, CancellationToken ct)
    {
        var query = _context.Clientes.AsQueryable();

        if (request.Estado.HasValue)
            query = query.Where(c => (int)c.Estado == request.Estado.Value);

        return await query
            .OrderBy(c => c.Nombre)
            .Select(c => new ClienteDto(
                c.ClienteId, c.Nombre, c.Telefono, c.Email, c.Cedula,
                c.Direccion, c.Credito, (int)c.Estado, c.Sexo,
                c.FechaNacimiento, c.FechaRegistro, (int)c.Tipo,(int)c.TipoEntidad))
            .ToListAsync(ct);
    }
}

// ── GET BY ID ───────────────────────────
public record GetClienteByIdQuery(int Id) : IRequest<ClienteDto>;

public class GetClienteByIdHandler : IRequestHandler<GetClienteByIdQuery, ClienteDto>
{
    private readonly IAppDbContext _context;
    public GetClienteByIdHandler(IAppDbContext context) => _context = context;

    public async Task<ClienteDto> Handle(GetClienteByIdQuery request, CancellationToken ct)
    {
        var c = await _context.Clientes.FindAsync([request.Id], ct)
            ?? throw new NotFoundException(nameof(Cliente), request.Id);

        return new ClienteDto(c.ClienteId, c.Nombre, c.Telefono, c.Email, c.Cedula,
            c.Direccion, c.Credito, (int)c.Estado, c.Sexo,
            c.FechaNacimiento, c.FechaRegistro, (int)c.Tipo, (int)c.TipoEntidad);
    }
}

// ── SEARCH (autocomplete) ───────────────
public record SearchClientesQuery(string Nombre) : IRequest<List<ClienteSearchDto>>;

public class SearchClientesHandler : IRequestHandler<SearchClientesQuery, List<ClienteSearchDto>>
{
    private readonly IAppDbContext _context;
    public SearchClientesHandler(IAppDbContext context) => _context = context;

    public async Task<List<ClienteSearchDto>> Handle(SearchClientesQuery request, CancellationToken ct)
    {
        return await _context.Clientes
            .Where(c => c.Nombre.StartsWith(request.Nombre))
            .OrderBy(c => c.Nombre)
            .Take(10)
            .Select(c => new ClienteSearchDto(c.ClienteId, c.Nombre, c.Telefono, c.Cedula))
            .ToListAsync(ct);
    }
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

// ── CREATE ──────────────────────────────
public record CrearClienteCommand(
    int EmpresaId,
    string Nombre,
    string? Telefono,
    string? Email,
    string? Cedula,
    string? Direccion,
    decimal Credito,
    bool Sexo,
    DateTime? FechaNacimiento,
    int Tipo,
    int TipoEntidad
) : IRequest<int>;

public class CrearClienteValidator : AbstractValidator<CrearClienteCommand>
{
    public CrearClienteValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(75).WithMessage("El nombre es requerido.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Credito).GreaterThanOrEqualTo(0);
    }
}

public class CrearClienteHandler : IRequestHandler<CrearClienteCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearClienteHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearClienteCommand request, CancellationToken ct)
    {
        var cliente = new Cliente
        {
            EmpresaId       = request.EmpresaId,
            Nombre          = request.Nombre,
            Telefono        = request.Telefono,
            Email           = request.Email,
            Cedula          = request.Cedula,
            Direccion       = request.Direccion,
            Credito         = request.Credito,
            Sexo            = request.Sexo,
            FechaNacimiento = request.FechaNacimiento,
            Tipo            = (TipoCliente)request.Tipo,
            Estado          = EstadoGeneral.Activo,
            FechaRegistro   = DateTime.Now,
            TipoEntidad     = (TipoEntidad)request.TipoEntidad
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(ct);
        return cliente.ClienteId;
    }
}

// ── UPDATE ──────────────────────────────
public record ActualizarClienteCommand(
    int ClienteId,
    string Nombre,
    string? Telefono,
    string? Email,
    string? Cedula,
    string? Direccion,
    decimal Credito,
    bool Sexo,
    DateTime? FechaNacimiento,
    int Tipo,
    int TipoEntidad
) : IRequest;

public class ActualizarClienteValidator : AbstractValidator<ActualizarClienteCommand>
{
    public ActualizarClienteValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(75);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.Credito).GreaterThanOrEqualTo(0);
    }
}

public class ActualizarClienteHandler : IRequestHandler<ActualizarClienteCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarClienteHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarClienteCommand request, CancellationToken ct)
    {
        var cliente = await _context.Clientes.FindAsync([request.ClienteId], ct)
            ?? throw new NotFoundException(nameof(Cliente), request.ClienteId);

        cliente.Nombre          = request.Nombre;
        cliente.Telefono        = request.Telefono;
        cliente.Email           = request.Email;
        cliente.Cedula          = request.Cedula;
        cliente.Direccion       = request.Direccion;
        cliente.Credito         = request.Credito;
        cliente.Sexo            = request.Sexo;
        cliente.FechaNacimiento = request.FechaNacimiento;
        cliente.Tipo            = (TipoCliente)request.Tipo;
        cliente.TipoEntidad     = (TipoEntidad)request.TipoEntidad;

        await _context.SaveChangesAsync(ct);
    }
}

// ── TOGGLE ESTADO ───────────────────────
public record ToggleEstadoClienteCommand(int ClienteId) : IRequest;

public class ToggleEstadoClienteHandler : IRequestHandler<ToggleEstadoClienteCommand>
{
    private readonly IAppDbContext _context;
    public ToggleEstadoClienteHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ToggleEstadoClienteCommand request, CancellationToken ct)
    {
        var cliente = await _context.Clientes.FindAsync([request.ClienteId], ct)
            ?? throw new NotFoundException(nameof(Cliente), request.ClienteId);

        cliente.Estado = cliente.Estado == EstadoGeneral.Activo
            ? EstadoGeneral.Inactivo
            : EstadoGeneral.Activo;

        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarClienteCommand(int ClienteId) : IRequest;

public class EliminarClienteHandler : IRequestHandler<EliminarClienteCommand>
{
    private readonly IAppDbContext _context;
    public EliminarClienteHandler(IAppDbContext context) => _context = context;

    public async Task Handle(EliminarClienteCommand request, CancellationToken ct)
    {
        var cliente = await _context.Clientes.FindAsync([request.ClienteId], ct)
            ?? throw new NotFoundException(nameof(Cliente), request.ClienteId);

        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(ct);
    }
}
