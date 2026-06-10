using SaleGz.Domain.Enums;

namespace SaleGz.Domain.Entities;

public class ComprobanteElectronica
{
    public int ComprobanteElectronicoId { get; set; }
    public int FacturaId { get; set; }
    public string NumeroComprobante { get; set; } = string.Empty;
    public string Xml { get; set; } = string.Empty;
    public string FirmaDigital { get; set; } = string.Empty;
    public string? TrackId { get; set; }
    public EstadoComprobante Estado { get; set; } = EstadoComprobante.Pendiente;
    public string? RespuestaDgii { get; set; }
    public int IntentosCuenta { get; set; } = 0;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaAceptacion { get; set; }
    public string? MensajeError { get; set; }

    // Navegación
    public Factura Factura { get; set; } = null!;
}

public class LogTransaccionDgii
{
    public int LogTransaccionDgiiId { get; set; }
    public int ComprobanteElectronicoId { get; set; }
    public string Accion { get; set; } = string.Empty;
    public string Peticion { get; set; } = string.Empty;
    public string? Respuesta { get; set; }
    public int CodigoHttpRespuesta { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Exitoso { get; set; }

    // Navegación
    public ComprobanteElectronica ComprobanteElectronica { get; set; } = null!;
}