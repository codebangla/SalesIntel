using SalesIntel.API.Application.CQRS;
using SalesIntel.API.Application.Services;

namespace SalesIntel.API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services;
    }
}
