namespace Application.Common.Interfaces.Policies;

public interface IInfiniteRetryPolicy<in TClass> : IPolicy<TClass> where TClass : class
{
}