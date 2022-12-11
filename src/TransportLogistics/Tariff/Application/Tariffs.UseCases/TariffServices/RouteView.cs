using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Маршрут
/// </summary>
public sealed class RouteView
{
    /// <summary>
    /// Тип маршрута
    /// </summary>
    /// <example>Auto</example>
    [Required]
    public RouteType Type { get; set; }

    /// <summary>
    /// Уникальный hash маршрута
    /// </summary>
    [Required]
    public string Hash { get; set; } = null!;

    /// <summary>
    /// Точки маршрута
    /// </summary>
    [Required]
    public IEnumerable<PointView> Points { get; set; } = null!;
}