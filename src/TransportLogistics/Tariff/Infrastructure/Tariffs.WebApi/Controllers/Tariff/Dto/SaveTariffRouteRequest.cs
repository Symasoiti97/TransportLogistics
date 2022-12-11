using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение маршрута тарифа
/// </summary>
[PublicAPI]
public sealed class SaveTariffRouteRequest
{
    /// <summary>
    /// Маршрут тарифа
    /// </summary>
    [Required]
    public RouteDto Route { get; set; } = null!;
}