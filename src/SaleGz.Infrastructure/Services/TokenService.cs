using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SaleGz.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config) => _config = config;

    public string GenerateToken(Usuario usuario)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        // Rol como string legible para [Authorize(Roles = "...")]
        new Claim(ClaimTypes.Role, usuario.TipoUsuario switch {
            TipoUsuario.Admin      => "Admin",
            TipoUsuario.SuperAdmin => "SuperAdmin",
            _                      => "Normal"
        }),
        // Valor numérico para el frontend y el controller
        new Claim("tipoUsuario", ((int)usuario.TipoUsuario).ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpirationHours"] ?? "8")),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
