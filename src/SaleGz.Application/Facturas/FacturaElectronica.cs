 
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SaleGz.Application.Common.Exceptions;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Application.Facturas.Service;
using SaleGz.Domain.Entities;
using SaleGz.Domain.Enums; 


namespace SaleGz.Application.Facturas;

// ════════════════════════════════════════
// DTOs
// ════════════════════════════════════════

public record ComprobanteElectronicoDto(
    int ComprobanteElectronicoId,
    int FacturaId,
    string NumeroComprobante,
    string Estado,
    string? TrackId,
    DateTime FechaCreacion,
    DateTime? FechaEnvio,
    DateTime? FechaAceptacion,
    string? MensajeError
);

// ════════════════════════════════════════
// QUERY: Obtener estado de comprobante
// ════════════════════════════════════════

public record ObtenerComprobanteElectronicoQuery(int FacturaId) : IRequest<ComprobanteElectronicoDto>;

public class ObtenerComprobanteElectronicoHandler : IRequestHandler<ObtenerComprobanteElectronicoQuery, ComprobanteElectronicoDto>
{
    private readonly IAppDbContext _context;

    public ObtenerComprobanteElectronicoHandler(IAppDbContext context) => _context = context;

    public async Task<ComprobanteElectronicoDto> Handle(ObtenerComprobanteElectronicoQuery request, CancellationToken ct)
    {
        var comprobante = await _context.ComprobantesElectronicos
            .FirstOrDefaultAsync(x => x.FacturaId == request.FacturaId, ct)
            ?? throw new NotFoundException(nameof(ComprobanteElectronica), request.FacturaId);

        return new ComprobanteElectronicoDto(
            comprobante.ComprobanteElectronicoId,
            comprobante.FacturaId,
            comprobante.NumeroComprobante,
            comprobante.Estado.ToString(),
            comprobante.TrackId,
            comprobante.FechaCreacion,
            comprobante.FechaEnvio,
            comprobante.FechaAceptacion,
            comprobante.MensajeError
        );
    }
}

// ════════════════════════════════════════
// COMMAND: Enviar comprobante a DGII
// ════════════════════════════════════════

public record EnviarComprobanteDgiiCommand(int FacturaId) : IRequest;

public class EnviarComprobanteDgiiHandler : IRequestHandler<EnviarComprobanteDgiiCommand>
{
    private readonly IAppDbContext _context; 
    private readonly IDgiiIntegrationService _dgiiService;
    private readonly IComprobanteXmlService _xmlService;
    private readonly IComprobanteRepository _repository;
    private readonly ILogger<EnviarComprobanteDgiiHandler> _logger;

    private readonly IXmlSignatureService _xmlSignatureService;

    public EnviarComprobanteDgiiHandler(
     IAppDbContext context,
     IDgiiIntegrationService dgiiService,
     IComprobanteXmlService xmlService,
     IXmlSignatureService xmlSignatureService,
     IComprobanteRepository repository,
     ILogger<EnviarComprobanteDgiiHandler> logger)
    {
        _context = context;
        _dgiiService = dgiiService;
        _xmlService = xmlService;
        _xmlSignatureService = xmlSignatureService;
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(EnviarComprobanteDgiiCommand request, CancellationToken ct)
    {
        var factura = await _context.Facturas
            .Include(x => x.Detalles).ThenInclude(d => d.Producto)
            .Include(x => x.ComprobanteElectronica)
            .FirstOrDefaultAsync(x => x.FacturaId == request.FacturaId, ct)
            ?? throw new NotFoundException(nameof(Factura), request.FacturaId);

        if (!factura.RequiereEcf)
        {
            throw new BusinessException("Esta factura no requiere facturación electrónica.");
        }

        if (factura.ComprobanteElectronica != null &&
            factura.ComprobanteElectronica.Estado == EstadoComprobante.Aceptado)
        {
            throw new BusinessException("El comprobante ya fue aceptado por DGII.");
        }

        try
        {
            _logger.LogInformation("Generando XML para factura {FacturaId}", request.FacturaId);
            var xmlGenerado = await _xmlService.GenerarXmlAsync(factura, ct);

            // Aquí se debería firmar digitalmente el XML
            // Por ahora, usamos un dummy
            //var firmaDigital = GenerarFirmaDigital();

            var xmlFirmado = await _xmlSignatureService
                .FirmarXmlAsync(xmlGenerado, ct);

            _logger.LogInformation("Enviando comprobante a DGII para factura {FacturaId}", request.FacturaId);
            var respuesta = await _dgiiService.EnviarComprobanteAsync(
                factura.CodigoSeguridad ?? "",
                xmlFirmado,
                string.Empty,
                "31",
                ct
            );

            if (!respuesta.IsSuccess)
            {
                var comprobante = factura.ComprobanteElectronica ?? new ComprobanteElectronica();
                comprobante.FacturaId = factura.FacturaId;
                comprobante.NumeroComprobante = factura.CodigoSeguridad ?? "";
                comprobante.Estado = EstadoComprobante.ErrorEnvio;
                comprobante.MensajeError = respuesta.Content;
                comprobante.FechaCreacion = DateTime.Now;

                if (factura.ComprobanteElectronica == null)
                {
                    await _repository.AgregarAsync(comprobante, ct);
                }
                else
                {
                    await _repository.ActualizarAsync(comprobante, ct);
                }

                throw new BusinessException($"Error al enviar comprobante: {respuesta.Content}");
            }

            // Guardar comprobante
            var nuevoComprobante = new ComprobanteElectronica
            {
                FacturaId = factura.FacturaId,
                NumeroComprobante = factura.CodigoSeguridad ?? "",
                Xml = xmlFirmado,
                FirmaDigital = "XML_FIRMADO",
                TrackId = respuesta.TrackId,
                Estado = EstadoComprobante.Enviado,
                RespuestaDgii = respuesta.Content,
                FechaCreacion = DateTime.Now,
                FechaEnvio = DateTime.Now
            };

            factura.FechaEnvioEcf = DateTime.Now;
            factura.ComprobanteElectronicoId = nuevoComprobante.ComprobanteElectronicoId;

            await _repository.AgregarAsync(nuevoComprobante, ct);

            _logger.LogInformation("Comprobante enviado exitosamente. TrackId: {TrackId}", respuesta.TrackId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar comprobante de factura {FacturaId}", request.FacturaId);
            throw;
        }
    }

    private string GenerarFirmaDigital()
    {
        // TODO: Implementar firma digital real con certificado
        // Por ahora retorna un dummy
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("firma_digital_dummy"));
    }
}

// ════════════════════════════════════════
// COMMAND: Reintentar envío de comprobante
// ════════════════════════════════════════

public record ReintentarEnvioComprobanteCommand(int ComprobanteElectronicoId) : IRequest;

public class ReintentarEnvioComprobanteHandler : IRequestHandler<ReintentarEnvioComprobanteCommand>
{
    private readonly IAppDbContext _context;
    private readonly IDgiiIntegrationService _dgiiService;
    private readonly IComprobanteRepository _repository;
    private readonly ILogger<ReintentarEnvioComprobanteHandler> _logger;

    public ReintentarEnvioComprobanteHandler(
        IAppDbContext context,
        IDgiiIntegrationService dgiiService,
        IComprobanteRepository repository,
        ILogger<ReintentarEnvioComprobanteHandler> logger)
    {
        _context = context;
        _dgiiService = dgiiService;
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(ReintentarEnvioComprobanteCommand request, CancellationToken ct)
    {
        var comprobante = await _context.ComprobantesElectronicos
            .FirstOrDefaultAsync(x => x.ComprobanteElectronicoId == request.ComprobanteElectronicoId, ct)
            ?? throw new NotFoundException(nameof(ComprobanteElectronica), request.ComprobanteElectronicoId);

        if (comprobante.IntentosCuenta >= 3)
        {
            throw new BusinessException("Se han alcanzado el máximo de reintentos permitidos.");
        }

        try
        {
            _logger.LogInformation("Reintentando envío de comprobante {NumeroComprobante}", comprobante.NumeroComprobante);

            var respuesta = await _dgiiService.EnviarComprobanteAsync(
                comprobante.NumeroComprobante,
                comprobante.Xml,
                comprobante.FirmaDigital,
                "31",
                ct
            );

            if (!respuesta.IsSuccess)
            {
                comprobante.IntentosCuenta++;
                comprobante.MensajeError = respuesta.Content;
                await _repository.ActualizarAsync(comprobante, ct);
                throw new BusinessException($"Error al reenviar: {respuesta.Content}");
            }

            comprobante.Estado = EstadoComprobante.Enviado;
            comprobante.TrackId = respuesta.TrackId;
            comprobante.RespuestaDgii = respuesta.Content;
            comprobante.FechaEnvio = DateTime.Now;
            comprobante.IntentosCuenta++;

            await _repository.ActualizarAsync(comprobante, ct);

            _logger.LogInformation("Reenvío exitoso. TrackId: {TrackId}", respuesta.TrackId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al reintentar envío de comprobante {NumeroComprobante}",
                comprobante.NumeroComprobante);
            throw;
        }
    }
 

}