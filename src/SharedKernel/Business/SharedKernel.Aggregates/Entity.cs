namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Entity
/// </summary>
/// <typeparam name="TKey">Entity type key</typeparam>
public abstract class Entity<TKey> where TKey : struct, IComparable
{
    /// <summary>
    /// Create <see cref="Entity{TKey}"/>
    /// </summary>
    /// <param name="id">Entity id</param>
    protected Entity(TKey id)
    {
        SetId(id);
    }

    /// <summary>
    /// Entity id
    /// </summary>
    public TKey Id { get; private set; }

    /// <summary>
    /// Set entity id
    /// </summary>
    /// <param name="id">Entity id</param>
    protected void SetId(TKey id)
    {
        Error.Throw().IfDefault(id);

        Id = id;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Id.GetHashCode();
    }

    /// <summary>
    /// Check equal between two object
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Entity<TKey>? left, Entity<TKey>? right)
    {
        return left?.Equals(right) ?? Equals(right, null);
    }

    /// <summary>
    /// Check not equal between two object
    /// </summary>
    /// <param name="left">Left object</param>
    /// <param name="right">Right object</param>
    /// <returns></returns>
    public static bool operator !=(Entity<TKey>? left, Entity<TKey>? right)
    {
        return !(left == right);
    }
}