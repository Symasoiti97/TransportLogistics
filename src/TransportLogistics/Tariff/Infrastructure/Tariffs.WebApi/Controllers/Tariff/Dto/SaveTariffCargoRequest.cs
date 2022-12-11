using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение тарифа с параметрами груза
/// </summary>
public sealed class SaveTariffCargoRequest
{
    /// <summary>
    /// Принадлежность контейнера
    /// </summary>
    [Required]
    public ContainerOwn ContainerOwn { get; set; }

    /// <summary>
    /// Тип груза
    /// </summary>
    [Required]
    public CargoType CargoType { get; set; }

    /// <summary>
    /// Размер контейнера
    /// </summary>
    [Required]
    public ContainerSize ContainerSize { get; set; }
}