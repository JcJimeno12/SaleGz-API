using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SaleGz.Application.Common.Behaviors;
using System.Reflection;

namespace SaleGz.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR — registra todos los Handlers automáticamente
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // FluentValidation — registra todos los Validators automáticamente
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
