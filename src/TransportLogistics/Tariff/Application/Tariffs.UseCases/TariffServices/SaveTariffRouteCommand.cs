using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Сохранить тариф с параметрами маршрута
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
/// <param name="Route">Маршрут</param>
public sealed record SaveTariffRouteCommand(Guid TariffId, Route Route) : ICommand;