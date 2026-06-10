using Microsoft.Extensions.Logging;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Entities;
using System.Xml.Linq;

namespace SaleGz.Infrastructure.Services;

public class ComprobanteXmlService : IComprobanteXmlService
{
    private readonly ILogger<ComprobanteXmlService> _logger;

    public ComprobanteXmlService(ILogger<ComprobanteXmlService> logger)
    {
        _logger = logger;
    }

    public async Task<string> GenerarXmlAsync(Factura factura, CancellationToken ct)
    {
        return await Task.Run(() =>
        {
            try
            {
                _logger.LogInformation("Generando XML para factura {FacturaId}", factura.FacturaId);

                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    new XElement("Comprobante",
                        new XElement("Header",
                            new XElement("EncF", factura.CodigoSeguridad ?? ""),
                            new XElement("FechaEmision", factura.Fecha.ToString("yyyy-MM-dd")),
                            new XElement("RncEmisor", ""),
                            new XElement("RazonSocialEmisor", ""),
                            new XElement("EncInteropEmisor", ""),
                            new XElement("DireccionEmisor", ""),
                            new XElement("RncComprador", factura.RncCliente ?? ""),
                            new XElement("RazonSocialComprador", factura.NombreCliente),
                            new XElement("DireccionComprador", "")
                        ),
                        new XElement("Body",
                            new XElement("Detalles",
                                factura.Detalles.Select((d, i) =>
                                    new XElement("Detalle",
                                        new XElement("NumeroLinea", i + 1),
                                        new XElement("NombreItem", d.Producto?.Descripcion ?? ""),
                                        new XElement("IndicadorBienServicio", 1),
                                        new XElement("Cantidad", d.Cantidad),
                                        new XElement("PrecioUnitario", d.SubTotal / d.Cantidad),
                                        new XElement("MontoItem", d.SubTotal + d.ITBS)
                                    )
                                )
                            )
                        ),
                        new XElement("Totales",
                            new XElement("MontoGravado", factura.Gravado),
                            new XElement("MontoGravadoI1", factura.Gravado),
                            new XElement("MontoGravadoI2", 0),
                            new XElement("MontoGravadoI3", 0),
                            new XElement("MontoExento", factura.Exento),
                            new XElement("TotalItbis", factura.ITBS),
                            new XElement("TotalItbis1", factura.ITBS),
                            new XElement("TotalItbis2", 0),
                            new XElement("TotalItbis3", 0),
                            new XElement("MontoTotal", factura.Total),
                            new XElement("ValorPagar", factura.Total)
                        )
                    )
                );

                var xmlString = doc.ToString();
                _logger.LogInformation("XML generado exitosamente para factura {FacturaId}", factura.FacturaId);

                return xmlString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar XML para factura {FacturaId}", factura.FacturaId);
                throw;
            }
        }, ct);
    }
}