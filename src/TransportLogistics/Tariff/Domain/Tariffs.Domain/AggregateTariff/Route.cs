using System.Collections.Immutable;
using Domain.Abstracts;

namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Маршрут
/// </summary>
public class Route : ValueObject
{
    /// <summary>
    /// Тип маршрута
    /// </summary>
    public RouteType Type { get; private set; }

    /// <summary>
    /// Точки маршрута
    /// </summary>
    public ImmutableArray<Point> Points => _points.ToImmutableArray();

    private Point[] _points = null!;

    public Route(Point[] points)
    {
        SetPoints(points);
        SetRouteType();
    }

    private void SetPoints(Point[] points)
    {
        if (!points.Any())
            throw new ArgumentException("Points can't be empty", nameof(points));

        _points = points;
    }

    private void SetRouteType()
    {
        Type = RouteType.Unknown;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return Points.Cast<object>().Append(Type);
    }
}