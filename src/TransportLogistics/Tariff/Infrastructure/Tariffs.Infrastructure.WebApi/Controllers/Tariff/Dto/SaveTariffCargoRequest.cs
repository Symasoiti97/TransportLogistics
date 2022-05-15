using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение тарифа с параметрами груза
/// </summary>
public class SaveTariffCargoRequest
{
    /// <summary>
    /// Принадлежность контейнера
    /// </summary>
    public ContainerOwn ContainerOwn { get; set; }

    /// <summary>
    /// Тип груза
    /// </summary>
    public CargoType CargoType { get; set; }

    /// <summary>
    /// Размер контейнера
    /// </summary>
    public ContainerSize ContainerSize { get; set; }
}