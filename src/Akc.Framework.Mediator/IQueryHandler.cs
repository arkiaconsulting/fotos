namespace Akc.Framework.Mediator;

/// <summary>
/// Represents a query that returns a result of type TResult.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IQuery<TResult>;

/// <summary>
/// Represents a handler for processing queries of type TQuery and returning a result of type TResult.
/// </summary>
/// <typeparam name="TQuery"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken);
}
