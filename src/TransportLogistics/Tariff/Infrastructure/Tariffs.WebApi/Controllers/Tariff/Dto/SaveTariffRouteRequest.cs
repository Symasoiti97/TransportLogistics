using System.ComponentModel.DataAnnotations;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение маршрута тарифа
/// </summary>
public sealed class SaveTariffRouteRequest
{
    /// <summary>
    /// Маршрут тарифа
    /// </summary>
    [Required]
    public RouteDto Route { get; set; } = null!;
}
