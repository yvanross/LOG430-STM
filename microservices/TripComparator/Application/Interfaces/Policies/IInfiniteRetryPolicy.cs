namespace Application.Interfaces.Policies;

public interface IInfiniteRetryPolicy<in TClass> : IPolicy<TClass> where TClass : class
{ }