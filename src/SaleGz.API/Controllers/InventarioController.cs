using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Inventario;

namespace SaleGz.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventarioController : ControllerBase
{
    private readonly IMediator _mediator;
    public InventarioController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtener todos los movimientos de inventario con filtros opcionales</summary>
    [HttpGet("movimientos")]
    public async Task<IActionResult> GetMovimientos(
        [FromQuery] int? productoId,
        [FromQuery] int? tipo,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin)
        => Ok(await _mediator.Send(new GetMovimientosInventarioQuery(productoId, tipo, fechaInicio, fechaFin)));

    /// <summary>Obtener movimiento por ID</summary>
    [HttpGet("movimientos/{id}")]
    public async Task<IActionResult> GetMovimientoById(int id)
        => Ok(await _mediator.Send(new GetMovimientoInventarioByIdQuery(id)));

    /// <summary>Obtener historial de movimientos de un producto</summary>
    [HttpGet("producto/{productoId}/historial")]
    public async Task<IActionResult> GetHistorialProducto(int productoId, [FromQuery] int? limit)
        => Ok(await _mediator.Send(new GetMovimientosProductoQuery(productoId, limit)));

    /// <summary>Crear movimiento de inventario (entrada, salida o ajuste)</summary>
    [HttpPost("movimientos")]
    public async Task<IActionResult> CrearMovimiento([FromBody] CrearMovimientoInventarioCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMovimientoById), new { id }, new { id });
    }
     
}
