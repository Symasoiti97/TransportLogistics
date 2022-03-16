namespace Domain.Abstracts;

public abstract class Entity<T> where T : IComparable
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    public T Id { get; private set; } = default!;

    protected Entity(T id)
    {
        SetId(id);
    }

    private void SetId(T id)
    {
        if (id.Equals(default(T)))
            throw new ArgumentException("Value can't be empty", nameof(id));

        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<T> item)
            return false;

        if (ReferenceEquals(this, item))
            return true;

        return item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        return left?.Equals(right) ?? Equals(right, null);
    }

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}