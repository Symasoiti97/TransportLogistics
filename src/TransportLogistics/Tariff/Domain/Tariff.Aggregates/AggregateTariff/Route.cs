using System.Collections.Immutable;
using Generator.Equals;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Маршрут
/// </summary>
[Equatable]
public sealed partial record Route
{
    private const byte _minPointCount = 2;

    /// <summary>
    /// Тип маршрута
    /// </summary>
    public RouteType Type { get; }

    /// <summary>
    /// Точки маршрута
    /// </summary>
    [SetEquality]
    public IImmutableSet<Point> Points { get; }

    /// <summary>
    /// Уникальный hash маршрута
    /// </summary>
    public string Hash => $"{string.Join("|", Points.Select(x => x.Hash))}|{Type}";

    /// <summary>
    /// Создать <see cref="Route" />
    /// </summary>
    /// <param name="points">Точки маршрута</param>
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
