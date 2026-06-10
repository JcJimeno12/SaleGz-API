using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Usuarios;

namespace SaleGz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsuariosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetUsuariosQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _mediator.Send(new GetUsuarioByIdQuery(id)));

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q) => Ok(await _mediator.Send(new SearchUsuariosQuery(q)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearUsuarioCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarUsuarioCommand command)
    {
        if (id != command.UsuarioId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id}/cambiar-contrasena")]
    public async Task<IActionResult> CambiarContrasena(int id, [FromBody] CambiarContrasenaCommand command)
    {
        if (id != command.UsuarioId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>Eliminar usuario</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarUsuarioCommand(id));
        return NoContent();
    }
}
