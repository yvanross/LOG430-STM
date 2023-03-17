namespace Entities.DomainInterfaces.ResourceManagement;

public interface IScheduler
{
    void TryAddTask(Func<Task> func);

}