using Application.Common.AntiCorruption;
using MediatR;

namespace Aspect.Configuration.AntiCorruption.Mediatr;

public class QueryProcessor<TQuery, TResult> : IRequestHandler<QueryWrapper<TQuery, TResult>, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _handler;

    public QueryProcessor(IQueryHandler<TQuery, TResult> handler)
    {
        _handler = handler;
    }

    public Task<TResult> Handle(QueryWrapper<TQuery, TResult> request, CancellationToken cancellationToken)
    {
        return _handler.Handle(request.Query);
    }
}