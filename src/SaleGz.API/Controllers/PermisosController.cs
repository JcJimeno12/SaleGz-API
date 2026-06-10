 
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Usuarios;
using SaleGz.Domain.Enums;

namespace SaleGz.API.Controllers;

[Authorize]
[ApiController]
[Route("api/usuarios/{usuarioId}/permisos")]
public class PermisosController : ControllerBase
{
    private readonly IMediator _mediator;
    public PermisosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(int usuarioId)
        => Ok(await _mediator.Send(new GetPermisosQuery(usuarioId)));

    [HttpPut] 
    public async Task<IActionResult> Update(int usuarioId, [FromBody] GuardarPermisosCommand command)
    {
        if (usuarioId != command.UsuarioId) return BadRequest();

        var tipoUsuarioClaim = User.FindFirst("tipoUsuario")?.Value;
        if (!int.TryParse(tipoUsuarioClaim, out var tipoUsuario))
            return Forbid();

        // Solo Admin (0) y SuperAdmin (2) pueden modificar permisos — Normal (1) no puede
        if (tipoUsuario == (int)TipoUsuario.Normal)
            return Forbid("No tiene permisos para realizar esta acción.");

        var objetivo = await _mediator.Send(new GetUsuarioByIdQuery(usuarioId));

        // Nadie puede editar permisos de un SuperAdmin
        if (objetivo.TipoUsuario == (int)TipoUsuario.SuperAdmin)
            return Forbid("No se puede editar permisos de un superadmin.");

        // Admin (0) solo puede editar permisos de Normales (1), no de otros Admins
        if (tipoUsuario == (int)TipoUsuario.Admin && objetivo.TipoUsuario != (int)TipoUsuario.Normal)
            return Forbid("Un administrador solo puede editar permisos de usuarios normales.");

        await _mediator.Send(command);
        return NoContent();
    }
    /// <summary>Eliminar permiso de usuario</summary>
    [HttpDelete("{usuarioPermisoId}")]
    public async Task<IActionResult> Delete(int usuarioPermisoId)
    {
        await _mediator.Send(new EliminarPermisoUsuarioCommand(usuarioPermisoId));
        return NoContent();
    }



}

