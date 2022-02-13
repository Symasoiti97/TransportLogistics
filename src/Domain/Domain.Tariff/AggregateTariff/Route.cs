using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

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
    public Point[] Points { get; private set; } = null!;

    public Route(Point[] points)
    {
        SetPoints(points);
        SetRouteType();
    }

    private void SetPoints(Point[] points)
    {
        if (!points.Any())
            throw new ArgumentException("Points can't be empty", nameof(points));

        Points = points;
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