using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Productos;

namespace SaleGz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtener todos los productos. Filtrar por categoría</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoriaId)
        => Ok(await _mediator.Send(new GetProductosQuery(categoriaId)));

    /// <summary>Obtener producto por ID</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => Ok(await _mediator.Send(new GetProductoByIdQuery(id)));

    /// <summary>Buscar productos por descripción (autocomplete)</summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
        => Ok(await _mediator.Send(new SearchProductosQuery(q)));

    /// <summary>Productos con stock bajo o en mínimo</summary>
    [HttpGet("stock-bajo")]
    public async Task<IActionResult> StockBajo()
        => Ok(await _mediator.Send(new GetProductosStockBajoQuery()));

    /// <summary>Crear nuevo producto</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearProductoCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Actualizar producto</summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarProductoCommand command)
    {
        if (id != command.ProductoId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>Eliminar producto</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarProductoCommand(id));
        return NoContent();
    }
}
