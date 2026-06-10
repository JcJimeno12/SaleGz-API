using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Clientes;

namespace SaleGz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ClientesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtener todos los clientes. Filtrar por estado: 0=Inactivo, 1=Activo</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? estado)
        => Ok(await _mediator.Send(new GetClientesQuery(estado)));

    /// <summary>Obtener cliente por ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _mediator.Send(new GetClienteByIdQuery(id)));

    /// <summary>Buscar clientes por nombre (autocomplete)</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
        => Ok(await _mediator.Send(new SearchClientesQuery(q)));

    /// <summary>Crear nuevo cliente</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearClienteCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Actualizar cliente</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarClienteCommand command)
    {
        if (id != command.ClienteId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>Activar / Desactivar cliente</summary>
    [HttpPatch("{id}/toggle-estado")]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        await _mediator.Send(new ToggleEstadoClienteCommand(id));
        return NoContent();
    }

    /// <summary>Eliminar cliente</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarClienteCommand(id));
        return NoContent();
    }
}
