namespace Application.QueryServices.ServiceInterfaces;

public interface IQueryContext
{
    IQueryable<T> GetData<T>() where T : class;
}