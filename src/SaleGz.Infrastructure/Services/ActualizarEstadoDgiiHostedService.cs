using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SaleGz.Application.Common.Interfaces;
using SaleGz.Domain.Enums;
using SaleGz.Infrastructure.Persistence;
using SaleGz.Infrastructure.Repositories;

namespace SaleGz.Infrastructure.Services;

public class ActualizarEstadoDgiiHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ActualizarEstadoDgiiHostedService> _logger;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(10));

    public ActualizarEstadoDgiiHostedService(
        IServiceProvider serviceProvider,
        ILogger<ActualizarEstadoDgiiHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Servicio de actualización DGII iniciado");

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ActualizarEstadosAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el servicio de actualización DGII");
            }
        }
    }

    private async Task ActualizarEstadosAsync(CancellationToken ct)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IComprobanteRepository>();
            var dgiiService = scope.ServiceProvider.GetRequiredService<IDgiiIntegrationService>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var comprobantesPendientes = await repository.ObtenerPendientesAsync(ct);

            if (!comprobantesPendientes.Any())
            {
                _logger.LogInformation("No hay comprobantes pendientes para actualizar");
                return;
            }

            _logger.LogInformation("Actualizando {Count} comprobantes pendientes", comprobantesPendientes.Count);

            foreach (var comprobante in comprobantesPendientes)
            {
                try
                {
                    var resultado = await dgiiService.ConsultarEstadoAsync(comprobante.NumeroComprobante, ct);

                    if (!resultado.IsSuccess)
                    {
                        _logger.LogWarning("No se pudo consultar estado de {NumeroComprobante}",
                            comprobante.NumeroComprobante);
                        continue;
                    }

                    // Mapear estado DGII
                    var nuevoEstado = MapearEstado(resultado.Estado);

                    if (nuevoEstado != comprobante.Estado)
                    {
                        comprobante.Estado = nuevoEstado;
                        comprobante.RespuestaDgii = resultado.Content;

                        if (nuevoEstado == EstadoComprobante.Aceptado)
                        {
                            comprobante.FechaAceptacion = DateTime.Now;
                            _logger.LogInformation("Comprobante {NumeroComprobante} aceptado por DGII",
                                comprobante.NumeroComprobante);
                        }
                        else if (nuevoEstado == EstadoComprobante.Rechazado)
                        {
                            comprobante.MensajeError = string.Join(", ", resultado.Mensajes);
                            comprobante.IntentosCuenta++;
                            _logger.LogWarning("Comprobante {NumeroComprobante} rechazado. Razones: {Razones}",
                                comprobante.NumeroComprobante, comprobante.MensajeError);
                        }

                        await repository.ActualizarAsync(comprobante, ct);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error actualizando comprobante {NumeroComprobante}",
                        comprobante.NumeroComprobante);
                }

                // Esperar un poco entre consultas para no sobrecargar la API
                await Task.Delay(500, ct);
            }

            _logger.LogInformation("Actualización de comprobantes completada");
        }
    }

    private EstadoComprobante MapearEstado(string? estadoDgii)
    {
        return estadoDgii?.ToLower() switch
        {
            "aceptado" => EstadoComprobante.Aceptado,
            "rechazado" =>  EstadoComprobante.Rechazado,
            "procesando" => EstadoComprobante.Enviado,
            _ => EstadoComprobante.Pendiente
        };
    }

    public override void Dispose()
    {
        _timer.Dispose();
        base.Dispose();
    }
}