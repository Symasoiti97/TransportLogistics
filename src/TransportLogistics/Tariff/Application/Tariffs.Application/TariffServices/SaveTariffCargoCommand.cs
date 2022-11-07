using Application.Abstracts;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Сохранить тариф с параметрами груза
/// </summary>
public sealed class SaveTariffCargoCommand : ICommand
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    public Guid TariffId { get; set; }

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