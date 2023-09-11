namespace Application.Queries.Seedwork;

public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken cancellation);
}