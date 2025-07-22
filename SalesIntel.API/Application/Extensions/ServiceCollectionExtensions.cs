using System.Reflection;
using SalesIntel.API.Application.CQRS;
using SalesIntel.API.Application.Services;

namespace SalesIntel.API.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Mediator
        services.AddScoped<IMediator, Mediator>();
        
        // Register Services
        services.AddScoped<IForecastingService, ForecastingService>();
        
        // Register all handlers
        var assembly = Assembly.GetExecutingAssembly();
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && 
                         (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                          i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && 
                           (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                            i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));

            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, handlerType);
            }
        }

        return services;
    }
}
