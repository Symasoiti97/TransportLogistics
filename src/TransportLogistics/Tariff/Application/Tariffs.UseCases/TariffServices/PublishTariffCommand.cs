using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Команда публикации тарифа
/// Переводит тариф из черновика в действующий (Создает копию)
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
public sealed record PublishTariffCommand(Guid TariffId) : ICommand;