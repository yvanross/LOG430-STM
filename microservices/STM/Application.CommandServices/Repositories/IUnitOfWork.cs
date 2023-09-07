namespace Application.CommandServices.Repositories;

public interface IUnitOfWork
{
    public Task SaveChangesAsync();
}