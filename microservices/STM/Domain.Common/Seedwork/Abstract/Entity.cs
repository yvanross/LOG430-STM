namespace Domain.Common.Seedwork.Abstract;

public abstract class Entity<T> : IEquatable<Entity<T>> where T : class
{
    public string Id { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity<T>);
    }

    public bool Equals(Entity<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}