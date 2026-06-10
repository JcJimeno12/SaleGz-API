using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text; 

namespace SaleGz.Infrastructure.Services;

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

public class DgiiIntegrationService : IDgiiIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DgiiIntegrationService> _logger;
    private readonly string _urlPost;
    private readonly string _urlGet;
    private readonly string _apiKey;

    public DgiiIntegrationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DgiiIntegrationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        var dgiiSettings = configuration.GetSection("DgiiSettings");
        _urlPost = dgiiSettings["UrlPost"]!;
        _urlGet = dgiiSettings["UrlGet"]!;
        _apiKey = dgiiSettings["ApiKey"]!;

        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<EnviarComprobanteResponse> EnviarComprobanteAsync(
        string numeroComprobante,
        string xml,
        string firmaDigital,
        string tipoEcf,
        CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Enviando comprobante {NumeroComprobante} a DGII", numeroComprobante);

            var payload = new { encf = numeroComprobante, xml, firmaDigital };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var request = new HttpRequestMessage(HttpMethod.Post, _urlPost))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content = content;

                var response = await _httpClient.SendAsync(request, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error al enviar comprobante. StatusCode: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    return new EnviarComprobanteResponse
                    {
                        IsSuccess = false,
                        StatusCode = (int)response.StatusCode,
                        Content = responseContent
                    };
                }

                var obj = JObject.Parse(responseContent);
                var estado = obj["estado"]?.ToString();
                var trackId = obj["track_id"]?.ToString();
                var urlDgii = obj["url_dgii"]?.ToString();

                _logger.LogInformation("Comprobante {NumeroComprobante} enviado exitosamente. TrackId: {TrackId}",
                    numeroComprobante, trackId);

                return new EnviarComprobanteResponse
                {
                    IsSuccess = true,
                    StatusCode = (int)response.StatusCode,
                    Content = responseContent,
                    EstadoFactura = estado,
                    TrackId = trackId,
                    Url = urlDgii
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al enviar comprobante {NumeroComprobante}", numeroComprobante);
            return new EnviarComprobanteResponse
            {
                IsSuccess = false,
                StatusCode = 0,
                Content = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al enviar comprobante {NumeroComprobante}", numeroComprobante);
            return new EnviarComprobanteResponse
            {
                IsSuccess = false,
                StatusCode = 500,
                Content = ex.Message
            };
        }
    }

    public async Task<ConsultarEstadoResponse> ConsultarEstadoAsync(
        string numeroComprobante,
        CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Consultando estado de {NumeroComprobante} en DGII", numeroComprobante);

            var url = $"{_urlGet}?encf={numeroComprobante}";

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Error al consultar estado. StatusCode: {StatusCode}", response.StatusCode);
                    return new ConsultarEstadoResponse
                    {
                        IsSuccess = false,
                        StatusCode = (int)response.StatusCode,
                        Content = responseContent
                    };
                }

                var obj = JsonConvert.DeserializeObject<RootResponse>(responseContent);

                if (obj?.Data == null)
                {
                    _logger.LogWarning("Respuesta vacía al consultar estado de {NumeroComprobante}", numeroComprobante);
                    return new ConsultarEstadoResponse
                    {
                        IsSuccess = false,
                        StatusCode = 200,
                        Content = "Respuesta vacía"
                    };
                }

                var estado = obj.Data.Dgii?.Estado ?? "Desconocido";
                var mensajes = obj.Data.Dgii?.Mensaje?.Select(m => m.Valor).ToList() ?? new List<string>();

                _logger.LogInformation("Estado de {NumeroComprobante}: {Estado}", numeroComprobante, estado);

                return new ConsultarEstadoResponse
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Content = responseContent,
                    Estado = estado,
                    Mensajes = mensajes,
                    TrackId = obj.Data.TrackId
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar estado de {NumeroComprobante}", numeroComprobante);
            return new ConsultarEstadoResponse
            {
                IsSuccess = false,
                StatusCode = 500,
                Content = ex.Message
            };
        }
    }
}

// ════════════════════════════════════════
// RESPONSE MODELS
// ════════════════════════════════════════

public class EnviarComprobanteResponse
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? EstadoFactura { get; set; }
    public string? TrackId { get; set; }
    public string? Url { get; set; }
}

public class ConsultarEstadoResponse
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Estado { get; set; }
    public List<string> Mensajes { get; set; } = new();
    public string? TrackId { get; set; }
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