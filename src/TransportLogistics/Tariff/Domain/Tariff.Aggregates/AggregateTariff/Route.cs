using System.Collections.Immutable;
using Generator.Equals;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

[Equatable]
public sealed partial record Route
{
    private const byte _minPointCount = 2;

    public RouteType Type { get; }

    [SetEquality]
    public IImmutableSet<Point> Points { get; }

    public string Hash => $"{string.Join("|", Points.Select(x => x.Hash))}|{Type}";

    public Route(IReadOnlyCollection<Point> points)
    {
        Points = EnsureThatPointIsValid(points);
        Type = DefineRouteType();
    }

    private static IImmutableSet<Point> EnsureThatPointIsValid(IReadOnlyCollection<Point> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        var sortedPoint = new HashSet<Point>(points.Count);
        var order = 1;
        foreach (var point in points.OrderBy(point => point.Order))
        {
            if (point.Order != order)
            {
                throw new Conflict().WithDetails($"Invalid point order {order}.");
            }

            sortedPoint.Add(point);
            order++;
        }

        if (order < _minPointCount)
        {
            throw new Conflict().WithDetails("Point count must be greater than or equals 2.");
        }

        return sortedPoint.ToImmutableHashSet();
    }

    // TODO: Реалзиовать установку типа маршрута и добавить валидацию
    private static RouteType DefineRouteType() => RouteType.Unknown;
}
