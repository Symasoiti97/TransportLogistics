﻿using Domain.Tariff.Abstracts;

namespace Domain.Tariff.AggregateTariff;

public class Route : ValueObject
{
    /// <summary>
    /// Тип маршрута
    /// </summary>
    public RouteType Type { get; private set; }

    /// <summary>
    /// Точки маршрута
    /// </summary>
    public Point[] Points { get; private set; }

    public Route(Point[] points)
    {
        SetPoints(points);
        SetRouteType();
    }

    private void SetPoints(Point[] points)
    {
        if (!points.Any())
            throw new ArgumentException("Points can be not empty", nameof(points));

        Points = points;
    }

    private void SetRouteType()
    {
        Type = RouteType.Undefined;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return Points.Cast<object>().Append(Type);
    }
}