using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaleGz.Application.Common.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace SaleGz.Infrastructure.Services;

public class DgiiIntegrationService : IDgiiIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DgiiIntegrationService> _logger;

    public DgiiIntegrationService(HttpClient httpClient, IConfiguration configuration, ILogger<DgiiIntegrationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<EnviarComprobanteResponse> EnviarComprobanteAsync(
        string numeroComprobante,
        string xml,
        string firmaDigital,
        string tipoEcf,
        CancellationToken ct)
    {
        // TODO: Implementar lógica de envío a DGII
        throw new NotImplementedException();
    }

    public async Task<ConsultarEstadoResponse> ConsultarEstadoAsync(
        string numeroComprobante,
        CancellationToken ct)
    {
        // TODO: Implementar lógica de consulta de estado a DGII
        throw new NotImplementedException();
    }
}
 



// ════════════════════════════════════════
// DGII MODELS
// ════════════════════════════════════════

public class RootResponse
{
    [JsonProperty("status")]
    public bool Status { get; set; }

    [JsonProperty("data")]
    public DgiiData? Data { get; set; }
}

public class DgiiData
{
    [JsonProperty("ecf_queue_id")]
    public int? EcfQueueId { get; set; }

    [JsonProperty("track_id")]
    public string TrackId { get; set; } = string.Empty;

    [JsonProperty("encf")]
    public string Encf { get; set; } = string.Empty;

    [JsonProperty("tipo_ecf")]
    public string TipoEcf { get; set; } = string.Empty;

    [JsonProperty("fecha_emision")]
    public string FechaEmision { get; set; } = string.Empty;

    [JsonProperty("monto_total")]
    public string MontoTotal { get; set; } = string.Empty;

    [JsonProperty("dgii")]
    public DgiiStatus? Dgii { get; set; }

    [JsonProperty("comprador")]
    public Comprador? Comprador { get; set; }
}

public class DgiiStatus
{
    [JsonProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonProperty("codigo_respuesta")]
    public string CodigoRespuesta { get; set; } = string.Empty;

    [JsonProperty("mensaje")]
    public List<DgiiMensaje>? Mensaje { get; set; }

    [JsonProperty("fecha_procesado")]
    public string FechaProcesado { get; set; } = string.Empty;

    [JsonProperty("intentos")]
    public int? Intentos { get; set; }
}

public class DgiiMensaje
{
    [JsonProperty("valor")]
    public string Valor { get; set; } = string.Empty;

    [JsonProperty("codigo")]
    public int? Codigo { get; set; }
}

public class Comprador
{
    [JsonProperty("rnc")]
    public string Rnc { get; set; } = string.Empty;

    [JsonProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("fecha")]
    public string Fecha { get; set; } = string.Empty;

    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("arecf")]
    public string? AreEcf { get; set; }
}