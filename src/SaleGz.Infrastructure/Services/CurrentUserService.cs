using Microsoft.AspNetCore.Http;
using SaleGz.Application.Common.Interfaces;
using System.Security.Claims;

namespace SaleGz.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public int UsuarioId { get; }
    public string Nombre { get; }
    public int TipoUsuario { get; }
    public bool IsAdmin => TipoUsuario == 0;


    public CurrentUserService(IHttpContextAccessor accessor)
    {
        var claims = accessor.HttpContext?.User;

        UsuarioId = int.Parse(
        claims?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
        );

        Nombre = claims?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        Nombre = claims?.FindFirst("nombre")?.Value ?? string.Empty;
        TipoUsuario = int.Parse(claims?.FindFirst("tipoUsuario")?.Value ?? "1");
    }
}
