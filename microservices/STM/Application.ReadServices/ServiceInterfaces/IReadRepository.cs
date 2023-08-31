namespace Application.QueryServices.ServiceInterfaces;

public interface IReadRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<T> GetAsync(string id);
}