using System.Collections.Immutable;
using Compress.Extensions;
using Domain.Abstracts;

namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Маршрут
/// </summary>
public sealed class Route : ValueObject
{
    /// <summary>
    /// Тип маршрута
    /// </summary>
    public RouteType Type { get; private set; }

    /// <summary>
    /// Точки маршрута
    /// </summary>
    public ImmutableArray<Point> Points => _points.ToImmutableArray();

    /// <summary>
    /// Уникальный hash маршрута
    /// </summary>
    public string Hash => $"{string.Join("|", _points.Select(x => x.Hash))}|{Type}".Compress();

    private Point[] _points = null!;

    public Route(Point[] points)
    {
        SetPoints(points);
        SetRouteType();
    }

    // TODO: Добавить валидацию на точки. (Проверять уникальность Order и коллекция должна быть в отсортированном виде)
    private void SetPoints(Point[] points)
    {
        if (!points.Any())
            throw new ArgumentException("Points can't be empty", nameof(points));

        _points = points;
    }

    // TODO: Реалзиовать установку типа маршрута и добавить валидацию
    private void SetRouteType()
    {
        Type = RouteType.Unknown;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return Points.Cast<object>().Append(Type);
    }
}