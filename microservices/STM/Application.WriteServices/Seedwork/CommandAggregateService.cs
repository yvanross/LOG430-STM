namespace Application.CommandServices.Seedwork;

public abstract class CommandAggregateService<T, TRepository> where TRepository : IWriteRepository<T>
{
    private readonly TRepository _writeRepository;

    public CommandAggregateService(TRepository writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public T Get(string id)
    {
        return _writeRepository.Get(id);
    }

    public void Save(T aggregate)
    {
        _writeRepository.Save(aggregate);
    }
}