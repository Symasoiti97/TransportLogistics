using JetBrains.Annotations;

namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Маршрут тарифа
/// </summary>
[PublicAPI]
public sealed class RouteDto
{
    /// <summary>
    /// Точки маршрута
    /// </summary>
    public PointDto[] Points { get; set; } = null!;
}