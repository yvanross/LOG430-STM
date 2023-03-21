namespace Entities.DomainInterfaces.ResourceManagement;

public interface IScheduler
{
    void TryAddTask(string name, Func<Task> func);

    void TryRemoveTask(string name);
}