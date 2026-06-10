using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;

namespace SaleGz.Application.Cuadres;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record CuadreDto(
    int CuadreId,
    int EmpresaId,
    int UsuarioId,
    string Usuario,
    DateTime Fecha,
    decimal Total,
    decimal P1, decimal P5, decimal P10, decimal P20, decimal P25,
    decimal P50, decimal P100, decimal P200, decimal P500, decimal P1000, decimal P2000,
    decimal TotalCheque, decimal TotalTarjeta, decimal TotalTransaccion,
    int CantidadCheque, int CantidadTarjeta, int CantidadTransaccion,
    decimal FondoInicial,
    decimal VentasSistema,
    decimal Diferencia,
    int EstadoCuadre,
    string? HoraApertura,
    string? HoraCierre,
    string? Notas
);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetCuadresQuery(DateTime? Desde = null, DateTime? Hasta = null)
    : IRequest<List<CuadreDto>>;

public class GetCuadresHandler : IRequestHandler<GetCuadresQuery, List<CuadreDto>>
{
    private readonly IAppDbContext _context;
    public GetCuadresHandler(IAppDbContext context) => _context = context;

    public async Task<List<CuadreDto>> Handle(GetCuadresQuery request, CancellationToken ct)
    {
        var query = _context.Cuadres.Include(c => c.Usuario).AsQueryable();

        if (request.Desde.HasValue)
            query = query.Where(c => c.Fecha >= request.Desde.Value.Date);
        if (request.Hasta.HasValue)
            query = query.Where(c => c.Fecha <= request.Hasta.Value.Date.AddDays(1).AddTicks(-1));

        return await query
            .OrderByDescending(c => c.Fecha)
            .Select(c => MapToDto(c))
            .ToListAsync(ct);
    }

    private static CuadreDto MapToDto(Cuadre c) => new(
        c.CuadreId, c.EmpresaId, c.UsuarioId, c.Usuario.Nombre, c.Fecha, c.Total,
        c.P1, c.P5, c.P10, c.P20, c.P25, c.P50, c.P100, c.P200, c.P500, c.P1000, c.P2000,
        c.TotalCheque, c.TotalTarjeta, c.TotalTransaccion,
        c.CantidadCheque, c.CantidadTarjeta, c.CantidadTransaccion,
        c.FondoInicial, c.VentasSistema, c.Diferencia, c.EstadoCuadre,
        c.HoraApertura, c.HoraCierre, c.Notas);
}

public record GetCuadreByIdQuery(int Id) : IRequest<CuadreDto>;

public class GetCuadreByIdHandler : IRequestHandler<GetCuadreByIdQuery, CuadreDto>
{
    private readonly IAppDbContext _context;
    public GetCuadreByIdHandler(IAppDbContext context) => _context = context;

    public async Task<CuadreDto> Handle(GetCuadreByIdQuery request, CancellationToken ct)
    {
        var c = await _context.Cuadres
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.CuadreId == request.Id, ct)
            ?? throw new NotFoundException(nameof(Cuadre), request.Id);

        return MapToDto(c);
    }

    private static CuadreDto MapToDto(Cuadre c) => new(
        c.CuadreId, c.EmpresaId, c.UsuarioId, c.Usuario.Nombre, c.Fecha, c.Total,
        c.P1, c.P5, c.P10, c.P20, c.P25, c.P50, c.P100, c.P200, c.P500, c.P1000, c.P2000,
        c.TotalCheque, c.TotalTarjeta, c.TotalTransaccion,
        c.CantidadCheque, c.CantidadTarjeta, c.CantidadTransaccion,
        c.FondoInicial, c.VentasSistema, c.Diferencia, c.EstadoCuadre,
        c.HoraApertura, c.HoraCierre, c.Notas);
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearCuadreCommand(
    int EmpresaId,
    int UsuarioId,
    decimal P1, decimal P5, decimal P10, decimal P20, decimal P25,
    decimal P50, decimal P100, decimal P200, decimal P500, decimal P1000, decimal P2000,
    decimal TotalCheque, decimal TotalTarjeta, decimal TotalTransaccion,
    int CantidadCheque, int CantidadTarjeta, int CantidadTransaccion,
    decimal FondoInicial,
    string? HoraApertura,
    string? HoraCierre,
    string? Notas
) : IRequest<int>;

public class CrearCuadreValidator : AbstractValidator<CrearCuadreCommand>
{
    public CrearCuadreValidator()
    {
        RuleFor(x => x.UsuarioId).GreaterThan(0);
    }
}

public class CrearCuadreHandler : IRequestHandler<CrearCuadreCommand, int>
{
    private readonly IAppDbContext _context;
    public CrearCuadreHandler(IAppDbContext context) => _context = context;

    public async Task<int> Handle(CrearCuadreCommand r, CancellationToken ct)
    {
        // Calcular ventas cobradas hoy por este usuario
        var hoy = DateTime.Now.Date;
        var manana = hoy.AddDays(1).AddTicks(-1);
        var ventasSistema = await _context.Facturas
            .Where(f => f.UsuarioId == r.UsuarioId
                     && f.Fecha >= hoy
                     && f.Fecha <= manana
                     && (int)f.Estado != 0)
            .SumAsync(f => f.Pagado, ct);

        var totalEfectivo = r.P1 + r.P5 + r.P10 + r.P20 + r.P25
                          + r.P50 + r.P100 + r.P200 + r.P500 + r.P1000 + r.P2000;

        var total = totalEfectivo + r.TotalCheque + r.TotalTarjeta + r.TotalTransaccion;
        var diferencia = total - r.FondoInicial - ventasSistema;
        var estado = diferencia == 0 ? 1 : 2; // 1=Aprobado, 2=ConDiferencia

        var cuadre = new Cuadre
        {
            EmpresaId = r.EmpresaId,
            UsuarioId = r.UsuarioId,
            Fecha = DateTime.Now,
            Total = total,
            P1 = r.P1,
            P5 = r.P5,
            P10 = r.P10,
            P20 = r.P20,
            P25 = r.P25,
            P50 = r.P50,
            P100 = r.P100,
            P200 = r.P200,
            P500 = r.P500,
            P1000 = r.P1000,
            P2000 = r.P2000,
            TotalCheque = r.TotalCheque,
            TotalTarjeta = r.TotalTarjeta,
            TotalTransaccion = r.TotalTransaccion,
            CantidadCheque = r.CantidadCheque,
            CantidadTarjeta = r.CantidadTarjeta,
            CantidadTransaccion = r.CantidadTransaccion,
            FondoInicial = r.FondoInicial,
            VentasSistema = ventasSistema,
            Diferencia = diferencia,
            EstadoCuadre = estado,
            HoraApertura = r.HoraApertura,
            HoraCierre = r.HoraCierre,
            Notas = r.Notas,
        };

        _context.Cuadres.Add(cuadre);
        await _context.SaveChangesAsync(ct);
        return cuadre.CuadreId;
    }
}

public record EliminarCuadreCommand(int CuadreId) : IRequest;

public class EliminarCuadreHandler : IRequestHandler<EliminarCuadreCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public EliminarCuadreHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(EliminarCuadreCommand request, CancellationToken ct)
    {
        if (_currentUser.TipoUsuario != 0)  // 0 = Admin
            throw new ForbiddenException();

        var cuadre = await _context.Cuadres.FindAsync([request.CuadreId], ct)
            ?? throw new NotFoundException(nameof(Cuadre), request.CuadreId);

        _context.Cuadres.Remove(cuadre);
        await _context.SaveChangesAsync(ct);
    }
}