namespace Application.ReadServices.Seedwork;

public interface IReadRepository<T>
{
    IEnumerable<T> GetAll();

    T Get(string id);
}