namespace Application.CommandServices.ServiceInterfaces.Repositories;

public interface IUnitOfWork
{
    public Task SaveChangesAsync();
}