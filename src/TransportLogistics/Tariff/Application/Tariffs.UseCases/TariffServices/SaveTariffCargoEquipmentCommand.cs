using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Сохранить тариф с параметрами груза
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
/// <param name="ContainerOwn">Принадлежность контейнера</param>
/// <param name="CargoType">Тип груза</param>
/// <param name="ContainerSize">Размер контейнера</param>
public sealed record SaveTariffCargoEquipmentCommand(
    Guid TariffId,
    ContainerOwn ContainerOwn,
    CargoType CargoType,
    ContainerSize ContainerSize) : ICommand;