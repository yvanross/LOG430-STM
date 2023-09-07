using Application.Queries.Seedwork;

namespace Configuration.Dispatchers;

public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        where TQuery : IQuery
    {
        var handler = _serviceProvider.CreateScope().ServiceProvider
            .GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();

        return await handler.Handle(query, cancellation);
    }
}