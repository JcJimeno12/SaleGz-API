using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Enums;

namespace SaleGz.Application.Auth;

// ── DTO de respuesta ─────────────────────────────────────────────
public record LoginResponse(
    string Token,
    int UsuarioId,
    string Nombre,
    int TipoUsuario
);

// ── Command ──────────────────────────────────────────────────────
public record LoginCommand(string Correo, string Contrasena) : IRequest<LoginResponse>;

public record VerifyPasswordCommand(string Contrasena) : IRequest<bool>;

// ── Validador ────────────────────────────────────────────────────
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("El correo es requerido.")
            .EmailAddress().WithMessage("El correo no es válido.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es requerida.");
    }
}
public class VerifyPasswordCommandValidator : AbstractValidator<VerifyPasswordCommand>
{
    public VerifyPasswordCommandValidator()
    {
        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es requerida.");
    }
}

// ── Handler ──────────────────────────────────────────────────────
public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;

    public LoginHandler(IAppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;

    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == request.Correo, cancellationToken)
            ?? throw new BusinessException("Credenciales incorrectas.");

        var passwordValida = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);

        if (!passwordValida)
            throw new BusinessException("Credenciales incorrectas.");

        var token = _tokenService.GenerateToken(usuario);

        return new LoginResponse(token, usuario.UsuarioId, usuario.Nombre, (int)usuario.TipoUsuario);
    }
}

// ── Handler ──────────────────────────────────────────────────────
public class VerifyPasswordHandler : IRequestHandler<VerifyPasswordCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public VerifyPasswordHandler(IAppDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(VerifyPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.UsuarioId == _currentUser.UsuarioId, cancellationToken)
            ?? throw new BusinessException("Usuario no encontrado.");

        // Solo admins y superadmins pueden autorizar cancelaciones
        if (!_currentUser.IsAdmin)
            throw new BusinessException("No tienes permisos de administrador para realizar esta acción.");

        var passwordValida = BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.Contrasena);
        if (!passwordValida)
            throw new BusinessException("Contraseña incorrecta.");

        return true;
    }
}
public record VerifyPinCommand(int Pin) : IRequest<bool>;

public class VerifyPinCommandValidator : AbstractValidator<VerifyPinCommand>
{
    public VerifyPinCommandValidator()
    {
        RuleFor(x => x.Pin)
            .GreaterThan(0).WithMessage("El PIN es requerido.")
            .InclusiveBetween(1000, 999999).WithMessage("El PIN debe ser de 4 a 6 dígitos.");
    }
}

public class VerifyPinHandler : IRequestHandler<VerifyPinCommand, bool>
{
    private readonly IAppDbContext _context;

    public VerifyPinHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(VerifyPinCommand request, CancellationToken cancellationToken)
    {
        var autorizado = await _context.Usuarios
            .AnyAsync(u =>
                (u.TipoUsuario == TipoUsuario.Admin || u.TipoUsuario == TipoUsuario.SuperAdmin)
                && u.Pin == request.Pin,
                cancellationToken);

        if (!autorizado)
            throw new BusinessException("PIN incorrecto.");

        return true;
    }
}