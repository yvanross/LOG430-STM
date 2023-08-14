namespace Application.Common.AntiCorruption;

public interface IQueryProcessor
{
    Task<TResult> Process<TResult>(IQuery<TResult> query);
}