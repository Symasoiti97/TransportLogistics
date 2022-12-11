using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Запрос на получения тарифа
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
public sealed record GetTariffQuery(Guid TariffId) : IQuery<TariffView>;