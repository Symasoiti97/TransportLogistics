using System.ComponentModel.Exceptions;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

public sealed record Point
{
    public Guid LocationId { get; private set; }

    public PointType Type { get; private set; }

    public ushort Order { get; private set; }

    public string Hash => $"{LocationId}|{Type}|{Order}";

    public Point(Guid locationId, PointType pointType, ushort order)
    {
        InvalidEnumArgumentException.ThrowIfUndefined(pointType);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(order);

        LocationId = locationId;
        Type = pointType;
        Order = order;
    }

    public static Point Fob(Guid locationId, ushort order) => new(locationId, PointType.Fob, order);

    public static Point For(Guid locationId, ushort order) => new(locationId, PointType.For, order);

    public static Point Fot(Guid locationId, ushort order) => new(locationId, PointType.Fot, order);
}
