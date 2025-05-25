namespace Akc.Framework.Mediator;

/// <summary>
/// Represents a sender that can send commands and queries to their respective handlers.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends a command to be handled by the appropriate handler.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result> Send(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command to be handled by the appropriate handler and returns a result.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<Result<TResult>> Send<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a query to be handled by the appropriate handler and returns a result.
    /// </summary>
    /// <param name="query">The query to send.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a result.</returns>
    Task<Result<TResult>> Send<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}