namespace Akc.Framework.Mediator;

/// <summary>
/// Represents a query that returns a result of type TResult.
/// </summary>
public interface ICommand;

/// <summary>
/// Represents a command handler that processes commands of type TCommand.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Represents a command that returns a result of type TResult.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface ICommand<TResult>;

/// <summary>
/// Represents a command handler that processes commands of type TCommand and returns a result of type TResult.
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the command and returns a result.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a result.</returns>
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
