namespace Application.Queries.Seedwork;

public interface IQuery<TResult>
{
    string GetQueryName();
}