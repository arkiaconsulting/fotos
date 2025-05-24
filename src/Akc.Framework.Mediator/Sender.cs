using Microsoft.Extensions.DependencyInjection;

namespace Akc.Framework.Mediator;

internal sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    async Task<Result> ISender.Send<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var handleMethod = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"Handler for command type {command.GetType().Name} does not implement Handle method.");

        var invoker = handleMethod.Invoke(handler, [command, cancellationToken]) as Task<Result>;

        return await invoker!;
    }

    Task<Result<TResult>> ISender.Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var handleMethod = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"Handler for command type {command.GetType().Name} does not implement Handle method.");

        var invoker = handleMethod.Invoke(handler, [command, cancellationToken]) as Task<Result<TResult>>;

        return invoker!;
    }

    async Task<Result<TResult>> ISender.Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var handleMethod = handlerType.GetMethod("Handle")
            ?? throw new InvalidOperationException($"Handler for query type {query.GetType().Name} does not implement Handle method.");

        var invoker = handleMethod.Invoke(handler, [query, cancellationToken]) as Task<Result<TResult>>;

        return await invoker!;
    }
}
