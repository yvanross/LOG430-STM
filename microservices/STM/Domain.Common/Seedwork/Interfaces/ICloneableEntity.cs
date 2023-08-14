namespace Domain.Common.Seedwork.Interfaces;

public interface ICloneableEntity<out T> where T : class
{
    T Clone();
}