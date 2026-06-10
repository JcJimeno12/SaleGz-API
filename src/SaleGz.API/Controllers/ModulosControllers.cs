using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Compras;
using SaleGz.Application.Cotizaciones;
using SaleGz.Application.Cuadres;
using SaleGz.Application.Facturas;
using SaleGz.Application.Gastos;
using SaleGz.Application.Reportes;
using SaleGz.Domain.Entities;

namespace SaleGz.API.Controllers;


// ════════════════════════════════════════
// COMPRAS
// ════════════════════════════════════════
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComprasController : ControllerBase
{
    private readonly IMediator _mediator;
    public ComprasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? estado) => Ok(await _mediator.Send(new GetComprasQuery(estado)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _mediator.Send(new GetCompraByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCompraCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPatch("{id}/saldar")]
    public async Task<IActionResult> Saldar(int id, [FromBody] SaldarCompraCommand command)
    {
        if (id != command.CompraId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _mediator.Send(new CancelarCompraCommand(id));
        return NoContent();
    }

    /// <summary>Eliminar compra</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarCompraCommand(id));
        return NoContent();
    }
}

// ════════════════════════════════════════
// COTIZACIONES
// ════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CotizacionesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CotizacionesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _mediator.Send(new GetCotizacionesQuery()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _mediator.Send(new GetCotizacionByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCotizacionCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id}/convertir")]
    public async Task<IActionResult> Convertir(int id, [FromBody] ConvertirCotizacionCommand command)
    {
        if (id != command.CotizacionId) return BadRequest("El ID de la cotización no coincide.");
        var facturaId = await _mediator.Send(command);
        return Ok(new { facturaId });
    }

    /// <summary>Eliminar cotización</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarCotizacionCommand(id));
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarCotizacionCommand command)
    {
        if (id != command.CotizacionId)
            return BadRequest("El ID de la cotización no coincide.");

        await _mediator.Send(command);
        return NoContent();
    }

}

// ════════════════════════════════════════
// GASTOS
// ════════════════════════════════════════
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GastosController : ControllerBase
{
    private readonly IMediator _mediator;
    public GastosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        => Ok(await _mediator.Send(new GetGastosQuery(desde, hasta)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _mediator.Send(new GetGastoByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearGastoCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("consumos")]
    public async Task<IActionResult> GetConsumos() => Ok(await _mediator.Send(new GetConsumosGastosQuery()));

    [HttpGet("consumos/search")]
    public async Task<IActionResult> SearchConsumos([FromQuery] string q) => Ok(await _mediator.Send(new SearchConsumosGastosQuery(q)));

    [HttpPost("consumos")]
    public async Task<IActionResult> CreateConsumo([FromBody] CrearConsumoGastoCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    /// <summary>Eliminar gasto</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarGastoCommand(id));
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarGastoCommand command)
    {
        if (id != command.GastosId)
            return BadRequest("El ID no coincide.");

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("consumos/{id}")]
    public async Task<IActionResult> UpdateConsumo(int id, [FromBody] ActualizarConsumoGastoCommand command)
    {
        if (id != command.ConsumosGastosId) return BadRequest("El ID no coincide.");
        await _mediator.Send(command);
        return NoContent();
    }

}

// ════════════════════════════════════════
// CUADRES
// ════════════════════════════════════════
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CuadresController : ControllerBase
{
    private readonly IMediator _mediator;
    public CuadresController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        => Ok(await _mediator.Send(new GetCuadresQuery(desde, hasta)));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _mediator.Send(new GetCuadreByIdQuery(id)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCuadreCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Eliminar cuadre</summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarCuadreCommand(id));
        return NoContent();
    }
}

// ════════════════════════════════════════
// REPORTES
// ════════════════════════════════════════
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReportesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("ventas")]
    public async Task<IActionResult> Ventas([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        => Ok(await _mediator.Send(new GetReporteVentasQuery(desde, hasta)));

    [HttpGet("cuentas-cobrar")]
    public async Task<IActionResult> CuentasCobrar()
        => Ok(await _mediator.Send(new GetReporteCuentasCobrarQuery()));

    [HttpGet("inventario")]
    public async Task<IActionResult> Inventario()
        => Ok(await _mediator.Send(new GetReporteInventarioQuery()));

    [HttpGet("gastos")]
    public async Task<IActionResult> Gastos([FromQuery] DateTime desde, [FromQuery] DateTime hasta)
        => Ok(await _mediator.Send(new GetReporteGastosQuery(desde, hasta)));

    [HttpGet("inventario/resumen")]
    public async Task<IActionResult> ResumenInventario()
    => Ok(await _mediator.Send(new GetResumenInventarioQuery()));
}


