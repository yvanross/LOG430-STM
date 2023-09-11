namespace Application.Queries.Seedwork;

public interface IQueryDispatcher
{
    Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        where TQuery : IQuery;
}