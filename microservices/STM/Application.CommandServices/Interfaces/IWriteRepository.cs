namespace Application.CommandServices.Interfaces;

public interface IWriteRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(params string[] ids);

    Task<T> GetAsync(string id);

    Task AddOrUpdateAsync(T aggregate);

    Task AddAllAsync(IEnumerable<T> aggregates);

    void Remove(T ride);

    Task ClearAsync();
}