using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Сохранить тариф с параметрами груза
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
/// <param name="CargoEquipment">Оборудование груза</param>
public sealed record SaveTariffCargoEquipmentCommand(Guid TariffId, CargoEquipment CargoEquipment) : IUseCase;