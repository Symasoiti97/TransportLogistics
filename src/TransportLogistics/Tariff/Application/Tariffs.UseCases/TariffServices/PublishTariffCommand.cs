using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Команда публикации тарифа
/// Переводит тариф из черновика в действующий (Создает копию)
/// </summary>
public sealed record PublishTariffCommand(Guid TariffId) : IUseCase;
