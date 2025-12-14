using ArgumentException = System.ComponentModel.Exceptions.ArgumentException;

namespace TL.SharedKernel.Business.Aggregates;

public abstract class Entity<TKey> where TKey : struct, IComparable
{
    public TKey Id { get; }

    protected Entity(TKey id)
    {
        ArgumentException.ThrowIfDefault(id);

        Id = id;
    }

    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right) =>
        left?.Equals(right) ?? Equals(right, objB: null);

    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right) => !(left == right);

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TKey> item)
        {
            return false;
        }

        if (ReferenceEquals(this, item))
        {
            return true;
        }

        return item.Id.Equals(Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
