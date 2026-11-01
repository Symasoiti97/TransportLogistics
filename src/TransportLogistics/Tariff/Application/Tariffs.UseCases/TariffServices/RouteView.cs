using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Route
/// </summary>
public sealed class RouteView
{
    /// <summary>
    /// Route type
    /// </summary>
    /// <example>Auto</example>
    [Required]
    public RouteType Type { get; set; }

    /// <summary>
    /// Unique route hash
    /// </summary>
    [Required]
    public string Hash { get; set; } = null!;

    /// <summary>
    /// Route points
    /// </summary>
    [Required]
    public IEnumerable<PointView> Points { get; set; } = null!;
}
