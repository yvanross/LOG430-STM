namespace Domain.Common.Seedwork.Interfaces;

public interface IValueObject<T> where T : struct, IEquatable<T> { }