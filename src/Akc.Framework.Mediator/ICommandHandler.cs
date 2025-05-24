namespace Akc.Framework.Mediator;

public interface ICommand;

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

public interface ICommand<TResult>;

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
