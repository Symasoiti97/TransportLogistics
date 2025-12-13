using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Маршрут тарифа
/// </summary>
public sealed class RouteDto
{
    /// <summary>
    /// Точки маршрута
    /// </summary>
    [Required]
    public PointDto[] Points { get; set; } = null!;
}
