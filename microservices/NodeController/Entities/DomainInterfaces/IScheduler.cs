namespace Entities.DomainInterfaces;

public interface IScheduler
{
    void TryAddTask(Func<Task> func);

}