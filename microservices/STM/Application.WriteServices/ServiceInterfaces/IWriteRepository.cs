namespace Application.CommandServices.ServiceInterfaces;

public interface IWriteRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<T> GetAsync(string id);

    Task<bool> Exists(string id);

    Task AddAsync(T aggregate);

    Task AddAllAsync(IEnumerable<T> aggregates);
}