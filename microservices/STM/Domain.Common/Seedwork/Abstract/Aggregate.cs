using Domain.Common.Seedwork.Interfaces;

namespace Domain.Common.Seedwork.Abstract;

public abstract class Aggregate<T> : Entity<T> where T : class { }