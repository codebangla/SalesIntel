namespace SalesIntel.API.Application.CQRS;

public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request);
    Task SendAsync(IRequest request);
}

public interface IRequest<TResponse>
{
}

public interface IRequest
{
}

public interface IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request);
}

public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    Task HandleAsync(TRequest request);
}
