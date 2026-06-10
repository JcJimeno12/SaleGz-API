using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Usuarios;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record UsuarioDto(
    int UsuarioId,
    int EmpresaId,
    string Nombre,
    string Correo,
    int TipoUsuario,
    int pin,
    DateTime FechaRegistro
);

public record UsuarioSearchDto(int UsuarioId, string Nombre);

// ════════════════════════════════════════
// QUERIES
// ════════════════════════════════════════

public record GetUsuariosQuery : IRequest<List<UsuarioDto>>;

public class GetUsuariosHandler : IRequestHandler<GetUsuariosQuery, List<UsuarioDto>>
{
    //private readonly IAppDbContext _context;
    public GetUsuariosHandler(IAppDbContext context, ICurrentUserService currentUser) => (_context, _currentUser) = (context, currentUser);

    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public async Task<List<UsuarioDto>> Handle(GetUsuariosQuery request, CancellationToken ct)
        => await _context.Usuarios
            .OrderBy(u => u.Nombre)
            .Select(u => new UsuarioDto(u.UsuarioId, u.EmpresaId, u.Nombre, u.Correo, (int)u.TipoUsuario, u.Pin, u.FechaRegistro))
            .ToListAsync(ct);
}

public record GetUsuarioByIdQuery(int Id) : IRequest<UsuarioDto>;

public class GetUsuarioByIdHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto>
{
    private readonly IAppDbContext _context;
    public GetUsuarioByIdHandler(IAppDbContext context) => _context = context;

    public async Task<UsuarioDto> Handle(GetUsuarioByIdQuery request, CancellationToken ct)
    {
        var u = await _context.Usuarios.FindAsync([request.Id], ct)
            ?? throw new NotFoundException(nameof(Usuario), request.Id);
        return new UsuarioDto(u.UsuarioId, u.EmpresaId, u.Nombre, u.Correo, (int)u.TipoUsuario, u.Pin, u.FechaRegistro);
    }
}

public record SearchUsuariosQuery(string Nombre) : IRequest<List<UsuarioSearchDto>>;

public class SearchUsuariosHandler : IRequestHandler<SearchUsuariosQuery, List<UsuarioSearchDto>>
{
    private readonly IAppDbContext _context;
    public SearchUsuariosHandler(IAppDbContext context) => _context = context;

    public async Task<List<UsuarioSearchDto>> Handle(SearchUsuariosQuery request, CancellationToken ct)
        => await _context.Usuarios
            .Where(u => u.Nombre.StartsWith(request.Nombre))
            .Take(10)
            .Select(u => new UsuarioSearchDto(u.UsuarioId, u.Nombre))
            .ToListAsync(ct);
}

// ════════════════════════════════════════
// COMMANDS
// ════════════════════════════════════════

public record CrearUsuarioCommand(
    int EmpresaId,
    string Nombre,
    string Correo,
    string Contrasena,
    int TipoUsuario,
    int Pin
) : IRequest<int>;

public class CrearUsuarioValidator : AbstractValidator<CrearUsuarioCommand>
{
    public CrearUsuarioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Correo)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Contrasena)
            .NotEmpty()
            .MinimumLength(4);

        RuleFor(x => x.TipoUsuario)
            .Must(v => Enum.IsDefined(typeof(TipoUsuario), v))
            .WithMessage("Tipo de usuario inválido.");

        RuleFor(x => x.Pin)
            .NotEmpty()
            .InclusiveBetween(1000, 9999) // PIN de 4 dígitos
            .WithMessage("El PIN debe ser de 4 dígitos.");
    }
}

public class CrearUsuarioHandler : IRequestHandler<CrearUsuarioCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CrearUsuarioHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<int> Handle(CrearUsuarioCommand request, CancellationToken ct)
    {
        var existe = await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo, ct);
        if (existe) throw new BusinessException("Ya existe un usuario con ese correo.");

        if ((TipoUsuario)_currentUser.TipoUsuario != TipoUsuario.SuperAdmin
        && request.TipoUsuario == (int)TipoUsuario.SuperAdmin)
       
        {
            throw new UnauthorizedAccessException("No puedes crear SuperAdmin");
        }

        var usuario = new Usuario
        {
            EmpresaId     = request.EmpresaId,
            Nombre        = request.Nombre,
            Correo        = request.Correo,
            Contrasena    = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            TipoUsuario   = (TipoUsuario)request.TipoUsuario,
            Pin           = request.Pin,
            FechaRegistro = DateTime.Now
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(ct);
        return usuario.UsuarioId;
    }
}

public record ActualizarUsuarioCommand(
    int UsuarioId,
    string Nombre,
    string Correo,
    int TipoUsuario,
    int Pin
) : IRequest;

public class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioCommand>
{
    public ActualizarUsuarioValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress();
    }
}

public class ActualizarUsuarioHandler : IRequestHandler<ActualizarUsuarioCommand>
{
    private readonly IAppDbContext _context;
    public ActualizarUsuarioHandler(IAppDbContext context) => _context = context;

    public async Task Handle(ActualizarUsuarioCommand request, CancellationToken ct)
    {
        var usuario = await _context.Usuarios.FindAsync([request.UsuarioId], ct)
            ?? throw new NotFoundException(nameof(Usuario), request.UsuarioId);

        usuario.Nombre      = request.Nombre;
        usuario.Correo      = request.Correo;
        usuario.TipoUsuario = (TipoUsuario)request.TipoUsuario;
        usuario.Pin         = request.Pin;

        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarUsuarioCommand(int UsuarioId) : IRequest;

public class EliminarUsuarioHandler : IRequestHandler<EliminarUsuarioCommand>
{
    private readonly IAppDbContext _context;
    public EliminarUsuarioHandler(IAppDbContext context) => _context = context;

    public async Task Handle(EliminarUsuarioCommand request, CancellationToken ct)
    {
        var usuario = await _context.Usuarios.FindAsync([request.UsuarioId], ct)
            ?? throw new NotFoundException(nameof(Usuario), request.UsuarioId);

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync(ct);
    }
}

public record CambiarContrasenaCommand(int UsuarioId, string ContrasenaActual, string NuevaContrasena) : IRequest;

public class CambiarContrasenaValidator : AbstractValidator<CambiarContrasenaCommand>
{
    public CambiarContrasenaValidator()
    {
        RuleFor(x => x.ContrasenaActual).NotEmpty();
        RuleFor(x => x.NuevaContrasena).NotEmpty().MinimumLength(4);
    }
}

public class CambiarContrasenaHandler : IRequestHandler<CambiarContrasenaCommand>
{
    private readonly IAppDbContext _context;
    public CambiarContrasenaHandler(IAppDbContext context) => _context = context;

    public async Task Handle(CambiarContrasenaCommand request, CancellationToken ct)
    {
        var usuario = await _context.Usuarios.FindAsync([request.UsuarioId], ct)
            ?? throw new NotFoundException(nameof(Usuario), request.UsuarioId);

        if (!BCrypt.Net.BCrypt.Verify(request.ContrasenaActual, usuario.Contrasena))
            throw new BusinessException("La contraseña actual es incorrecta.");

        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena);
        await _context.SaveChangesAsync(ct);
    }
}

public record EliminarPermisoUsuarioCommand(int UsuarioPermisoId) : IRequest;

public class EliminarPermisoUsuarioHandler : IRequestHandler<EliminarPermisoUsuarioCommand>
{
    private readonly IAppDbContext _context;
    public EliminarPermisoUsuarioHandler(IAppDbContext context) => _context = context;

    public async Task Handle(EliminarPermisoUsuarioCommand request, CancellationToken ct)
    {
        var permiso = await _context.UsuarioPermisos
            .FirstOrDefaultAsync(x => x.UsuarioPermisoId == request.UsuarioPermisoId, ct)
            ?? throw new NotFoundException(nameof(UsuarioPermiso), request.UsuarioPermisoId);

        _context.UsuarioPermisos.Remove(permiso);
        await _context.SaveChangesAsync(ct);
    }
}
