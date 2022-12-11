namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Value object
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Equal value object
    /// </summary>
    /// <param name="left">Left value object</param>
    /// <param name="right">Right value object</param>
    /// <returns>true if objects are equal, else false</returns>
    protected static bool EqualOperator(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
        {
            return false;
        }

        return ReferenceEquals(left, null) || left.Equals(right);
    }

    /// <summary>
    /// Not equal value object
    /// </summary>
    /// <param name="left">Left value object</param>
    /// <param name="right">Right value object</param>
    /// <returns>false if objects are equal, else true</returns>
    protected static bool NotEqualOperator(ValueObject? left, ValueObject? right)
    {
        return !EqualOperator(left, right);
    }

    /// <summary>
    /// Get quality components
    /// </summary>
    /// <returns>Quality components</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject) obj;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GetEqualityComponents().Select(x => x.GetHashCode()).Aggregate((x, y) => x ^ y);
    }
}