using System.Collections.Concurrent;
using System.Reflection;

namespace SalesIntel.API.Application.CQRS;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Mediator> _logger;
    private static readonly ConcurrentDictionary<Type, Type> _handlerCache = new();

    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Processing request: {RequestType}", request.GetType().Name);

        var requestType = request.GetType();
        var handlerType = GetHandlerType(requestType, typeof(IRequestHandler<,>));

        if (handlerType == null)
            throw new InvalidOperationException($"No handler found for request type {requestType.Name}");

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler {handlerType.Name} not registered in DI container");

        try
        {
            var method = handlerType.GetMethod("HandleAsync");
            var result = await (Task<TResponse>)method!.Invoke(handler, new object[] { request })!;
            
            _logger.LogInformation("Successfully processed request: {RequestType}", request.GetType().Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request: {RequestType}", request.GetType().Name);
            throw;
        }
    }

    public async Task SendAsync(IRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger.LogInformation("Processing request: {RequestType}", request.GetType().Name);

        var requestType = request.GetType();
        var handlerType = GetHandlerType(requestType, typeof(IRequestHandler<>));

        if (handlerType == null)
            throw new InvalidOperationException($"No handler found for request type {requestType.Name}");

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"Handler {handlerType.Name} not registered in DI container");

        try
        {
            var method = handlerType.GetMethod("HandleAsync");
            await (Task)method!.Invoke(handler, new object[] { request })!;
            
            _logger.LogInformation("Successfully processed request: {RequestType}", request.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request: {RequestType}", request.GetType().Name);
            throw;
        }
    }

    private Type? GetHandlerType(Type requestType, Type handlerInterfaceType)
    {
        return _handlerCache.GetOrAdd(requestType, _ =>
        {
            var responseType = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                ?.GetGenericArguments()[0];

            if (responseType != null)
            {
                return handlerInterfaceType.MakeGenericType(requestType, responseType);
            }

            if (requestType.GetInterfaces().Any(i => i == typeof(IRequest)))
            {
                return handlerInterfaceType.MakeGenericType(requestType);
            }

            return null!;
        });
    }
}
