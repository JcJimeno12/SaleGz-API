// SaleGz.Application.Common.Interfaces/IDgiiIntegrationService.cs
namespace SaleGz.Application.Common.Interfaces;

public record EnviarComprobanteResponse
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? EstadoFactura { get; init; }
    public string? TrackId { get; init; }
    public string? Url { get; init; }
}

public record ConsultarEstadoResponse
{
    public bool IsSuccess { get; init; }
    public int StatusCode { get; init; }
    public string Content { get; init; } = string.Empty;
    public string? Estado { get; init; }
    public List<string> Mensajes { get; init; } = new();
    public string? TrackId { get; init; }
}

public interface IDgiiIntegrationService
{
    Task<EnviarComprobanteResponse> EnviarComprobanteAsync(
        string numeroComprobante,
        string xml,
        string firmaDigital,
        string tipoEcf,
        CancellationToken ct);

    Task<ConsultarEstadoResponse> ConsultarEstadoAsync(
        string numeroComprobante,
        CancellationToken ct);
}