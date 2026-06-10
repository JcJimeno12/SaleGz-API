using SaleGz.Domain.Entities;

namespace SaleGz.Application.Common.Interfaces;

/// <summary>
/// Servicio para generar XML de comprobantes electrónicos según especificaciones DGII
/// </summary>
public interface IComprobanteXmlService
{
    /// <summary>
    /// Genera el XML de un comprobante electrónico a partir de una factura
    /// </summary>
    Task<string> GenerarXmlAsync(Factura factura, CancellationToken ct);
}

/// <summary>
/// Repositorio para operaciones de comprobantes electrónicos
/// </summary>
public interface IComprobanteRepository
{
    /// <summary>
    /// Obtiene un comprobante por ID de factura
    /// </summary>
    Task<ComprobanteElectronica?> ObtenerPorFacturaAsync(int facturaId, CancellationToken ct);

    /// <summary>
    /// Obtiene un comprobante por número de comprobante
    /// </summary>
    Task<ComprobanteElectronica?> ObtenerPorNumeroComprobanteAsync(string numeroComprobante, CancellationToken ct);

    /// <summary>
    /// Obtiene un comprobante por TrackId
    /// </summary>
    Task<ComprobanteElectronica?> ObtenerPorTrackIdAsync(string trackId, CancellationToken ct);

    /// <summary>
    /// Obtiene todos los comprobantes pendientes de envío o confirmación
    /// </summary>
    Task<List<ComprobanteElectronica>> ObtenerPendientesAsync(CancellationToken ct);

    /// <summary>
    /// Obtiene todos los comprobantes rechazados
    /// </summary>
    Task<List<ComprobanteElectronica>> ObtenerRechazadosAsync(CancellationToken ct);

    /// <summary>
    /// Agrega un nuevo comprobante electrónico
    /// </summary>
    Task AgregarAsync(ComprobanteElectronica comprobante, CancellationToken ct);

    /// <summary>
    /// Actualiza un comprobante electrónico existente
    /// </summary>
    Task ActualizarAsync(ComprobanteElectronica comprobante, CancellationToken ct);

    /// <summary>
    /// Registra un log de transacción DGII
    /// </summary>
    Task RegistrarLogAsync(LogTransaccionDgii log, CancellationToken ct);
}
