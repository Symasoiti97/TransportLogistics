using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

public class Point : ValueObject
{
    /// <summary>
    /// Локация
    /// </summary>
    public Location Location { get; private set; }

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

    private void SetOrder(int order)
    {
        if (order <= 0) throw new ArgumentOutOfRangeException(nameof(order));

        Order = order;
    }

    private void SetPointType(PointType pointType)
    {
        Type = pointType;
    }

    private void SetLocation(Location location)
    {
        Location = location;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new[] {Location as object, Order, Type};
    }
}