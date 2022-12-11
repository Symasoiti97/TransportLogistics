using System.Collections.Immutable;
using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Маршрут
/// </summary>
public sealed class Route : ValueObject
{
    private Point[] _points = null!;

    /// <summary>
    /// Создать <see cref="Route"/>
    /// </summary>
    /// <param name="points">Точки маршрута</param>
    public Route(Point[] points)
    {
        SetPoints(points);
        SetRouteType();
    }

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
    public string Hash => $"{string.Join("|", _points.Select(x => x.Hash))}|{Type}";

    // TODO: Добавить валидацию на точки. (Проверять уникальность Order и коллекция должна быть в отсортированном виде)
    private void SetPoints(Point[] points)
    {
        Error.Throw().IfEmpty(points);

        _points = points;
    }

    // TODO: Реалзиовать установку типа маршрута и добавить валидацию
    private void SetRouteType()
    {
        Type = RouteType.Unknown;
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return Points.Cast<object>().Append(Type);
    }
}