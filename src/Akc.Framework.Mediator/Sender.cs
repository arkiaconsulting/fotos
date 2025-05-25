using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Akc.Framework.Mediator;

internal sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;
    private readonly HandlerMethodCache _handlerMethodCache;
    private readonly HandlerTypeCache _handlerTypeCache;

    public Sender(
        IServiceProvider serviceProvider,
        HandlerMethodCache handlerMethodCache,
        HandlerTypeCache handlerTypeCache)
    {
        _serviceProvider = serviceProvider;
        _handlerMethodCache = handlerMethodCache;
        _handlerTypeCache = handlerTypeCache;
    }

    async Task<Result> ISender.Send(ICommand command, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var invoker = GetHandlerInvoker<ICommandHandler<ICommand>, Result>(
            command.GetType(), nameof(ICommandHandler<>.Handle), scope.ServiceProvider);

        return await invoker(command, cancellationToken);
    }

    async Task<Result<TResult>> ISender.Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var invoker = GetHandlerInvoker<ICommandHandler<ICommand<TResult>, TResult>, Result<TResult>>(
            command.GetType(), typeof(TResult), nameof(ICommandHandler<,>.Handle), scope.ServiceProvider);

        return await invoker(command, cancellationToken);
    }

    async Task<Result<TResult>> ISender.Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var invoker = GetHandlerInvoker<IQueryHandler<IQuery<TResult>, TResult>, Result<TResult>>(
            query.GetType(), typeof(TResult), nameof(IQueryHandler<,>.Handle), scope.ServiceProvider);

        return await invoker(query, cancellationToken);
    }

    #region Private

    private Func<object, object, Task<TReturn>> GetHandlerInvoker<THandler, TReturn>(Type requestType, string methodName, IServiceProvider serviceProvider)
        where TReturn : IResult
    {
        var handlerType = _handlerTypeCache.GetOrAdd(requestType, key =>
            typeof(THandler)
                .GetGenericTypeDefinition()
                .MakeGenericType(key)
        );

        var handler = serviceProvider.GetRequiredService(handlerType);

        var method = _handlerMethodCache.GetOrAdd(handlerType, key =>
            key.GetMethod(methodName)
            ?? throw new InvalidOperationException($"Handler for command type '{requestType.Name}' does not implement '{methodName}' method."));

        return BuildInvoker<TReturn>(method, handler);
    }

    private Func<object, object, Task<TReturn>> GetHandlerInvoker<THandler, TReturn>(Type requestType, Type resultType, string methodName, IServiceProvider serviceProvider)
        where TReturn : IResult
    {
        var handlerType = _handlerTypeCache.GetOrAdd(requestType, key =>
            typeof(THandler)
                .GetGenericTypeDefinition()
                .MakeGenericType(key, resultType)
        );

        var handler = serviceProvider.GetRequiredService(handlerType);

        var method = _handlerMethodCache.GetOrAdd(handlerType, key =>
            key.GetMethod(methodName)
            ?? throw new InvalidOperationException($"Handler for command type '{requestType.Name}' does not implement '{methodName}' method."));

        return BuildInvoker<TReturn>(method, handler);
    }

    private static Func<object, object, Task<TReturn>> BuildInvoker<TReturn>(MethodInfo method, object handler) =>
        (request, cancellationToken) =>
        {
            var task = method.Invoke(handler, [request, cancellationToken]) as Task<TReturn>;

            return task ?? throw new InvalidOperationException($"Handler for command type '{request.GetType().Name}' does not return a valid task.");
        };

    #endregion Private
}
