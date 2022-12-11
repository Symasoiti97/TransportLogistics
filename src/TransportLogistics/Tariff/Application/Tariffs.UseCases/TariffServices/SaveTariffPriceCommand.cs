using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Команда сохранения тарифа с параметрами цены
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
/// <param name="Price">Цена</param>
public sealed record SaveTariffPriceCommand(Guid TariffId, Price Price) : ICommand;