using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Tariff route
/// </summary>
public sealed class RouteDto
{
    /// <summary>
    /// Route points
    /// </summary>
    [Required]
    public PointDto[] Points { get; set; } = null!;
}
