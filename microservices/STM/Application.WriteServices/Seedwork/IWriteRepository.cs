namespace Application.WriteServices.Seedwork;

public interface IWriteRepository<T>
{
    T Get(string id);

    void Save(T aggregate);
}