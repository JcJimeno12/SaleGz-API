using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaleGz.Application.Facturas.Repositories;
using SaleGz.Application.Facturas.Services;
using SaleGz.Infrastructure.Persistence;
using SaleGz.Infrastructure.Repositories;
using SaleGz.Infrastructure.Services;

namespace SaleGz.Infrastructure
{
    /// <summary>
    /// Configuración de inyección de dependencias para Infrastructure
    /// Registra implementaciones concretas de servicios y repositorios
    /// </summary>
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ===== BASE DE DATOS =====

            /// <summary>
            /// Registrar DbContext
            /// Usa connection string de appsettings
            /// </summary>
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelaySeconds: 5,
                            errorNumbersToAdd: null
                        );
                    }
                )
            );

            // ===== IMPLEMENTACIONES DE SERVICIOS =====

            /// <summary>
            /// Implementación concreta del servicio de facturación
            /// Se inyecta en controladores y otros servicios
            /// </summary>
            services.AddScoped<FacturaElectronicaService>(provider =>
                new FacturaElectronicaService(
                    provider.GetRequiredService<IFacturaRepository>(),
                    provider.GetRequiredService<INotaCreditoRepository>(),
                    provider.GetRequiredService<ICalculadoraImpuestosService>(),
                    provider.GetRequiredService<IValidadorComprobanteService>(),
                    provider.GetRequiredService<IServicioPacioliDgii>(),
                    provider.GetRequiredService<IControlEmpresaRepository>(),
                    provider.GetRequiredService<IConsumoElectronicoRepository>(),
                    provider.GetRequiredService<IFiscalElectronicoRepository>(),
                    provider.GetRequiredService<IGubernamentalElectronicoRepository>(),
                    provider.GetRequiredService<IRegimenEspecialElectronicoRepository>(),
                    provider.GetRequiredService<IMapper>()
                )
            );

            services.AddScoped<NotaCreditoService>();
            services.AddScoped<CalculadoraImpuestosService>();
            services.AddScoped<ValidadorComprobanteService>();
            services.AddScoped<ServicioPacioliDgii>();

            // ===== IMPLEMENTACIONES DE REPOSITORIOS =====

            services.AddScoped<FacturaRepository>();
            services.AddScoped<NotaCreditoRepository>();
            services.AddScoped<ControlEmpresaRepository>();
            services.AddScoped<ConsumoElectronicoRepository>();
            services.AddScoped<FiscalElectronicoRepository>();
            services.AddScoped<GubernamentalElectronicoRepository>();
            services.AddScoped<RegimenEspecialElectronicoRepository>();

            // ===== SERVICIOS AUXILIARES =====

            /// <summary>
            /// Servicio para generar correlativos/ENCF
            /// </summary>
            services.AddScoped<ICorrelativoService, CorrelativoService>();

            /// <summary>
            /// Servicio para logging de transacciones DGII
            /// Auditoría de envíos y respuestas
            /// </summary>
            services.AddScoped<ILogTransaccionDgiiService, LogTransaccionDgiiService>();

            /// <summary>
            /// Servicio para construir XML de comprobantes
            /// Convierte C# a formato DGII
            /// </summary>
            services.AddScoped<IConstructorXmlComprobanteService, ConstructorXmlComprobanteService>();

            // ===== HOSTED SERVICES =====

            /// <summary>
            /// Servicio en background que actualiza estados de facturas pendientes
            /// Se ejecuta cada 5 minutos
            /// </summary>
            services.AddHostedService<ActualizarEstadoFacturasHostedService>();

            /// <summary>
            /// Servicio que reintenta facturas rechazadas
            /// Se ejecuta cada hora
            /// </summary>
            services.AddHostedService<ReintentarFacturasRechazadasHostedService>();

            return services;
        }
    }
}