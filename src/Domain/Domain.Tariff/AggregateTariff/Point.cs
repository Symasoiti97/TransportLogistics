using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

/// <summary>
/// Точка маргрута
/// </summary>
public class Point : ValueObject
{
    /// <summary>
    /// Локация
    /// </summary>
    public Location Location { get; private set; } = null!;

    /// <summary>
    /// Тип точки
    /// </summary>
    public PointType Type { get; private set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    public int Order { get; private set; }

    public Point(Location location, PointType pointType, int order)
    {
        SetLocation(location);
        SetPointType(pointType);
        SetOrder(order);
    }

    public static Point Fob(Location location, int order)
    {
        return new Point(location, PointType.Fob, order);
    }

    public static Point For(Location location, int order)
    {
        return new Point(location, PointType.For, order);
    }

    public static Point Fot(Location location, int order)
    {
        return new Point(location, PointType.Fot, order);
    }

    private void SetOrder(int order)
    {
        if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));

        Order = order;
    }

    private void SetPointType(PointType pointType)
    {
        if (pointType == PointType.Undefined)
            throw new ArgumentException("Value can't be undefined", nameof(pointType));

        Type = pointType;
    }

    private void SetLocation(Location location)
    {
        Location = location;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new object[] {Location, Order, Type};
    }
}