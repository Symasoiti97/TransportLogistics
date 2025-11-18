using System.ComponentModel.Exceptions;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Точка маргрута
/// </summary>
public sealed class Point : ValueObject
{
    /// <summary>
    /// Локация
    /// </summary>
    public Guid LocationId { get; private set; }

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
    public string Hash => $"{LocationId}|{Type}|{Order}";

    /// <summary>
    /// Создать <see cref="Point" />
    /// </summary>
    /// <param name="locationId">Идентификатор локации</param>
    /// <param name="pointType">Тип точки</param>
    /// <param name="order">Порядковый номер</param>
    public Point(Guid locationId, PointType pointType, ushort order)
    {
        SetLocationId(locationId);
        SetPointType(pointType);
        SetOrder(order);
    }

    /// <summary>
    /// Создать точку с типом <see cref="PointType.Fob" />
    /// </summary>
    /// <param name="locationId">Идентификатор локации</param>
    /// <param name="order">Порядковый номер</param>
    /// <returns>Точка</returns>
    public static Point Fob(Guid locationId, ushort order) => new(locationId, PointType.Fob, order);

    /// <summary>
    /// Создать точку с типом <see cref="PointType.For" />
    /// </summary>
    /// <param name="locationId">Идентификатор локации</param>
    /// <param name="order">Порядковый номер</param>
    /// <returns>Точка</returns>
    public static Point For(Guid locationId, ushort order) => new(locationId, PointType.For, order);

    /// <summary>
    /// Создать точку с типом <see cref="PointType.Fot" />
    /// </summary>
    /// <param name="locationId">Идентификатор локации</param>
    /// <param name="order">Порядковый номер</param>
    /// <returns>Точка</returns>
    public static Point Fot(Guid locationId, ushort order) => new(locationId, PointType.Fot, order);

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents() => [LocationId, Order, Type];

    private void SetOrder(ushort order)
    {
        Order = order;
    }

    private void SetPointType(PointType pointType)
    {
        InvalidEnumArgumentException.ThrowIfUndefined(pointType);

        Type = pointType;
    }

    private void SetLocationId(Guid locationId)
    {
        LocationId = locationId;
    }
}