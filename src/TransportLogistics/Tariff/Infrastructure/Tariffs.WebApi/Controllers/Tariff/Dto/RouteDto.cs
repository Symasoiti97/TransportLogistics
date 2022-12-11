using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Маршрут тарифа
/// </summary>
[PublicAPI]
public sealed class RouteDto
{
    /// <summary>
    /// Точки маршрута
    /// </summary>
    [Required]
    public PointDto[] Points { get; set; } = null!;
}