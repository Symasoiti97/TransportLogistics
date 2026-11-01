using System.ComponentModel.DataAnnotations;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Request to save a tariff route
/// </summary>
public sealed class SaveTariffRouteRequest
{
    /// <summary>
    /// Tariff route
    /// </summary>
    [Required]
    public RouteDto Route { get; set; } = null!;
}
