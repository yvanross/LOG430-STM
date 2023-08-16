namespace Application.Common.Interfaces.Policies;

public interface IPolicy<in TClass> where TClass : class
{
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);

    Task ExecuteAsync(Func<Task> action);
}