using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Empresa;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record EmpresaDto(
    int EmpresaId,
    string Nombre,
    string Correo,
    string Direccion,
    string Telefono,
    string? Instagram,
    string? Facebook,
    string? Whatsapp,
    string? RNC,
    int TipoEmpresa,
    DateTime FechaRegistro
);

public record ControlDto(
    int ControlesId,
    int LimiteDiaPago,
    int TamanoFactura,
    decimal ITBS,
    int LimiteConteoFiscal,
    int LimiteConteoConsumo,
    int ItbsCalculo,
    bool MostrarTelefonoCliente,
    bool MostrarDireccionCliente,
    int NumOrden,
    int TipoOrden,
    int LimiteConteoRegimenEspecial,
    int LimiteConteoGubernamental,
    DateTime? FechaLimiteFiscal,
    DateTime? FechaLimiteConsumo,
    DateTime? FechaLimiteGubernamental,
    DateTime? FechaLimiteRegimenEspecial
);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetEmpresaQuery : IRequest<EmpresaDto>;

public class GetEmpresaHandler : IRequestHandler<GetEmpresaQuery, EmpresaDto>
{
    private readonly IAppDbContext _context;
    public GetEmpresaHandler(IAppDbContext context) => _context = context;

    public async Task<EmpresaDto> Handle(GetEmpresaQuery request, CancellationToken ct)
    {
        var e = await _context.Empresas.FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Empresa", 1);

        return new EmpresaDto(e.EmpresaId, e.Nombre, e.Correo, e.Direccion, e.Telefono,
            e.Instagram, e.Facebook, e.Whatsapp, e.RNC, (int)e.TipoEmpresa, e.FechaRegistro);
    }
}

public record GetControlQuery : IRequest<ControlDto>;

public class GetControlHandler : IRequestHandler<GetControlQuery, ControlDto>
{
    private readonly IAppDbContext _context;
    public GetControlHandler(IAppDbContext context) => _context = context;

    public async Task<ControlDto> Handle(GetControlQuery request, CancellationToken ct)
    {
        var c = await _context.Controles
            .FirstOrDefaultAsync(x => x.EmpresaId == 1, ct);

        if (c == null)
        {
            c = new Control
            {
                EmpresaId = 1, 
                NumOrden = 0,
                LimiteDiaPago = 0,
                TamanoFactura = 0,
                ITBS = 18,
                LimiteConteoFiscal = 0,
                LimiteConteoConsumo = 0,
                ItbsCalculo = 0,
                MostrarTelefonoCliente = false,
                MostrarDireccionCliente = false,
                TipoOrden = 0,
                LimiteConteoRegimenEspecial = 0,
                LimiteConteoGubernamental = 0
            };

            _context.Controles.Add(c);
            await _context.SaveChangesAsync(ct);
        }

        return new ControlDto(
            c.ControlesId,
            c.LimiteDiaPago,
            (int)c.TamanoFactura,
            c.ITBS,
            c.LimiteConteoFiscal,
            c.LimiteConteoConsumo,
            (int)c.ItbsCalculo,
            c.MostrarTelefonoCliente,
            c.MostrarDireccionCliente,
            c.NumOrden,
            c.TipoOrden,
            c.LimiteConteoRegimenEspecial,
            c.LimiteConteoGubernamental,
            c.FechaLimiteFiscal,
            c.FechaLimiteConsumo,
            c.FechaLimiteGubernamental,
            c.FechaLimiteRegimenEspecial
        );
    }
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record ActualizarEmpresaCommand(
    string Nombre,
    string Correo,
    string Direccion,
    string Telefono,
    string? Instagram,
    string? Facebook,
    string? Whatsapp,
    string? RNC,
    int TipoEmpresa
) : IRequest;

public class ActualizarEmpresaValidator : AbstractValidator<ActualizarEmpresaCommand>
{
    public ActualizarEmpresaValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Correo).EmailAddress().When(x => !string.IsNullOrEmpty(x.Correo));
    }
}

public class ActualizarEmpresaHandler : IRequestHandler<ActualizarEmpresaCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarEmpresaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarEmpresaCommand request, CancellationToken ct)
    {
        var empresa = await _context.Empresas.FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Empresa", 1);

        empresa.Nombre     = request.Nombre;
        empresa.Correo     = request.Correo;
        empresa.Direccion  = request.Direccion;
        empresa.Telefono   = request.Telefono;
        empresa.Instagram  = request.Instagram;
        empresa.Facebook   = request.Facebook;
        empresa.Whatsapp   = request.Whatsapp;
        empresa.RNC        = request.RNC;
        empresa.TipoEmpresa = (TipoEmpresa)request.TipoEmpresa;

        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarEmpresaCommand : IRequest;

public class EliminarEmpresaHandler : IRequestHandler<EliminarEmpresaCommand>
{
    private readonly IAppDbContext _context;
    public EliminarEmpresaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(EliminarEmpresaCommand request, CancellationToken ct)
    {
        var empresa = await _context.Empresas.FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException("Empresa", 1);

        _context.Empresas.Remove(empresa);
        await _context.SaveChangesAsync(ct);
    }
}

public record ActualizarControlCommand(
    int LimiteDiaPago,
    int TamanoFactura,
    decimal ITBS,
    int LimiteConteoFiscal,
    int LimiteConteoConsumo,
    int ItbsCalculo,
    bool MostrarTelefonoCliente,
    bool MostrarDireccionCliente,
    int TipoOrden,
    int LimiteConteoRegimenEspecial,
    int LimiteConteoGubernamental,
    DateTime? FechaLimiteFiscal,
    DateTime? FechaLimiteConsumo,
    DateTime? FechaLimiteGubernamental,
    DateTime? FechaLimiteRegimenEspecial
) : IRequest;





public class ActualizarControlHandler : IRequestHandler<ActualizarControlCommand>
{


    private readonly IAppDbContext _context;
    public ActualizarControlHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarControlCommand request, CancellationToken ct)
    {
        var control = await _context.Controles
    .FirstOrDefaultAsync(x => x.EmpresaId == 1, ct);

        if (control == null)
        {
            control = new Control
            {
                EmpresaId = 1,
                NumOrden = 0
            };

            _context.Controles.Add(control);
        }

        control.LimiteDiaPago               = request.LimiteDiaPago;
        control.TamanoFactura               = (TamanoFactura)request.TamanoFactura;
        control.ITBS                        = request.ITBS;
        control.LimiteConteoFiscal          = request.LimiteConteoFiscal;
        control.LimiteConteoConsumo         = request.LimiteConteoConsumo;
        control.ItbsCalculo                 = (ItbsCalculo)request.ItbsCalculo;
        control.MostrarTelefonoCliente      = request.MostrarTelefonoCliente;
        control.MostrarDireccionCliente     = request.MostrarDireccionCliente;
        control.TipoOrden                   = request.TipoOrden;
        control.LimiteConteoRegimenEspecial = request.LimiteConteoRegimenEspecial;
        control.LimiteConteoGubernamental   = request.LimiteConteoGubernamental;
        control.FechaLimiteFiscal           = request.FechaLimiteFiscal;
        control.FechaLimiteConsumo          = request.FechaLimiteConsumo;
        control.FechaLimiteGubernamental    = request.FechaLimiteGubernamental;
        control.FechaLimiteRegimenEspecial  = request.FechaLimiteRegimenEspecial;

        await _context.SaveChangesAsync(ct);
    }


}


