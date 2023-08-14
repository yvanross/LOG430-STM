using Application.Common.AntiCorruption;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Aspect.Configuration.AntiCorruption.Mediatr;

public class QueryWrapper<TQuery, TResult> : IRequest<TResult> where TQuery : IQuery<TResult>
{
    public TQuery Query { get; }

    public QueryWrapper(TQuery query)
    {
        Query = query;
    }
}