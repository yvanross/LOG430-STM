namespace Application.Interfaces.Policies;

public interface IBackOffRetryPolicy<in TClass> : IPolicy<TClass> where TClass : class
{}