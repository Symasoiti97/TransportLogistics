using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Маршрут
/// </summary>
public sealed class Route : ValueObject
{
    private const byte MinPointCount = 2;

    /// <summary>
    /// Создать <see cref="Route" />
    /// </summary>
    /// <param name="points">Точки маршрута</param>
    public Route(IReadOnlyCollection<Point> points)
    {
        Points = EnsureThatPointIsValid(points);
        Type = DefineRouteType();
    }

    /// <summary>
    /// Тип маршрута
    /// </summary>
    public RouteType Type { get; }

    /// <summary>
    /// Точки маршрута
    /// </summary>
    public IReadOnlySet<Point> Points { get; }

    /// <summary>
    /// Уникальный hash маршрута
    /// </summary>
    public string Hash => $"{string.Join("|", Points.Select(x => x.Hash))}|{Type}";

    private static IReadOnlySet<Point> EnsureThatPointIsValid(IReadOnlyCollection<Point> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        var sortedPoint = new HashSet<Point>(points.Count);
        var order = 1;
        foreach (var point in points.OrderBy(point => point.Order))
        {
            if (point.Order != order)
            {
                throw new Conflict("Invalid point order");
            }

            sortedPoint.Add(point);
            order++;
        }

        if (order < MinPointCount)
        {
            throw new Conflict("Point count must be greater than or equals 2");
        }

        return sortedPoint;
    }

    // TODO: Реалзиовать установку типа маршрута и добавить валидацию
    private static RouteType DefineRouteType()
    {
        return RouteType.Unknown;
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return Points.Cast<object>().Append(Type);
    }
}