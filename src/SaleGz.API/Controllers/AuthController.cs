using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Auth;

namespace SaleGz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Login — retorna JWT token</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    /// <summary>Verificar contraseña — para acciones sensibles</summary>
    /// <param name="command">Comando de verificación de contraseña</param>
    /// <returns>Resultado de la verificación</returns>
    [HttpPost("verify-password")]
    public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("verify-pin")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyPin([FromBody] VerifyPinCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

}