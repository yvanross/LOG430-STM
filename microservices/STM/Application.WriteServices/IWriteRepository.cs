namespace Application.CommandServices;

public interface IWriteRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync(params string[] ids);

    Task<T> GetAsync(string id);

    Task<bool> AnyAsync();

    Task AddAsync(T aggregate);

    Task AddAllAsync(IEnumerable<T> aggregates);
}