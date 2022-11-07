using JetBrains.Annotations;

namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение маршрута тарифа
/// </summary>
[PublicAPI]
public sealed class SaveTariffRouteRequest
{
    /// <summary>
    /// Маршрут тарифа
    /// </summary>
    public RouteDto Route { get; set; } = null!;
}