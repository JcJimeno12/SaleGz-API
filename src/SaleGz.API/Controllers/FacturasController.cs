using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Facturas;

namespace SaleGz.API.Controllers
{

    // ════════════════════════════════════════
    // FACTURAS
    // ════════════════════════════════════════

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FacturasController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FacturasController(IMediator mediator) => _mediator = mediator;

         
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? estado,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int? usuarioId,
            [FromQuery] int? tipoComprobante)
            => Ok(await _mediator.Send(new GetFacturasQuery(estado, desde, hasta, usuarioId, tipoComprobante)));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _mediator.Send(new GetFacturaByIdQuery(id)));

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q) => Ok(await _mediator.Send(new SearchFacturasQuery(q)));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearFacturaCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPatch("{id}/cancelar")]
        public async Task<IActionResult> Cancelar(int id)
        {
            await _mediator.Send(new CancelarFacturaCommand(id));
            return NoContent();
        }

        [HttpPatch("{id}/saldar")]
        public async Task<IActionResult> Saldar(int id, [FromBody] SaldarFacturaCommand command)
        {
            if (id != command.FacturaId) return BadRequest("El ID no coincide.");
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>Eliminar factura</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new EliminarFacturaCommand(id));
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActualizarFacturaCommand command)
        {
            if (id != command.FacturaId) return BadRequest("El ID no coincide.");
            await _mediator.Send(command);
            return NoContent();
        }


        // ✅ NUEVO - ENDPOINTS FACTURACIÓN ELECTRÓNICA

        /// <summary>Obtener estado de comprobante electrónico</summary>
        [HttpGet("{id}/comprobante")]
        public async Task<IActionResult> ObtenerComprobante(int id)
        {
            var dto = await _mediator.Send(new ObtenerComprobanteElectronicoQuery(id));
            return Ok(dto);
        }

        /// <summary>Enviar factura a DGII para generar e-CF</summary>
        [HttpPost("{id}/enviar-dgii")]
        public async Task<IActionResult> EnviarDgii(int id)
        {
            await _mediator.Send(new EnviarComprobanteDgiiCommand(id));
            return Accepted();
        }

        /// <summary>Reintentar envío de comprobante rechazado</summary>
        [HttpPost("comprobante/{comprobanteId}/reintentar")]
        public async Task<IActionResult> ReintentarComprobante(int comprobanteId)
        {
            await _mediator.Send(new ReintentarEnvioComprobanteCommand(comprobanteId));
            return Accepted();
        }



    }
}
