namespace Application.CommandServices.Seedwork;

public interface IWriteRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();

    Task<T> GetAsync(string id);

    Task AddAsync(T aggregate);
}