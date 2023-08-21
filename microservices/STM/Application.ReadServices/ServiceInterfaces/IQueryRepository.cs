namespace Application.QueryServices.ServiceInterfaces;

public interface IQueryRepository
{
    IQueryable<T> GetData<T>() where T : class;

}