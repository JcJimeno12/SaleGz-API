using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Categorias;

namespace SaleGz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtener todas las categorías. Filtrar por estado: 0=Inactivo, 1=Activo</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? estado)
        => Ok(await _mediator.Send(new GetCategoriasQuery(estado)));

    /// <summary>Obtener categoría por ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _mediator.Send(new GetCategoriaByIdQuery(id)));

    /// <summary>Crear nueva categoría</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCategoriaCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Actualizar categoría</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarCategoriaCommand command)
    {
        if (id != command.CategoriaId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>Activar / Desactivar categoría</summary>
    [HttpPatch("{id}/toggle-estado")]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        await _mediator.Send(new ToggleEstadoCategoriaCommand(id));
        return NoContent();
    }

    /// <summary>Eliminar categoría</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarCategoriaCommand(id));
        return NoContent();
    }
}
