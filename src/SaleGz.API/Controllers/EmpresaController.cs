using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleGz.Application.Empresa;

namespace SaleGz.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpresaController : ControllerBase
{
    private readonly IMediator _mediator;
    public EmpresaController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _mediator.Send(new GetEmpresaQuery()));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ActualizarEmpresaCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("control")]
    public async Task<IActionResult> GetControl() => Ok(await _mediator.Send(new GetControlQuery()));

    [HttpPut("control")]
    public async Task<IActionResult> UpdateControl([FromBody] ActualizarControlCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>Eliminar empresa</summary>
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        await _mediator.Send(new EliminarEmpresaCommand());
        return NoContent();
    }
}
