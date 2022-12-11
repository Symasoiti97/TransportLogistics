using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Точка маргрута
/// </summary>
public sealed class Point : ValueObject
{
    /// <summary>
    /// Создать <see cref="Point"/>
    /// </summary>
    /// <param name="location">Локация</param>
    /// <param name="pointType">Тип точки</param>
    /// <param name="order">Порядковый номер</param>
    public Point(Location location, PointType pointType, ushort order)
    {
        SetLocation(location);
        SetPointType(pointType);
        SetOrder(order);
    }

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
    public ushort Order { get; private set; }

    /// <summary>
    /// Уникальный hash точки
    /// </summary>
    public string Hash => $"{Location.Id}|{Type}|{Order}";

    /// <summary>
    /// Создать точку с типом <see cref="PointType.Fob"/>
    /// </summary>
    /// <param name="location">Локация</param>
    /// <param name="order">Порядковый номер</param>
    /// <returns>Точка</returns>
    public static Point Fob(Location location, ushort order)
    {
        return new Point(location, PointType.Fob, order);
    }

    /// <summary>
    /// Создать точку с типом <see cref="PointType.For"/>
    /// </summary>
    /// <param name="location">Локация</param>
    /// <param name="order">Порядковый номер</param>
    /// <returns>Точка</returns>
    public static Point For(Location location, ushort order)
    {
        return new Point(location, PointType.For, order);
    }

    /// <summary>
    /// Создать точку с типом <see cref="PointType.Fot"/>
    /// </summary>
    /// <param name="location">Локация</param>
    /// <param name="order">Порядковый номер</param>
    /// <returns>Точка</returns>
    public static Point Fot(Location location, ushort order)
    {
        return new Point(location, PointType.Fot, order);
    }

    private void SetOrder(ushort order)
    {
        Order = order;
    }

    private void SetPointType(PointType pointType)
    {
        Error.Throw().IfUndefined(pointType);

        Type = pointType;
    }

    private void SetLocation(Location location)
    {
        Location = location;
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new object[] {Location, Order, Type};
    }
}