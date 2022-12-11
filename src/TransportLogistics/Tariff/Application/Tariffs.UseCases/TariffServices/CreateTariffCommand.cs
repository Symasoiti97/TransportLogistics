using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Команда для создания тарифа
/// </summary>
/// <param name="TariffId">Идентификатор тарфиа</param>
public sealed record CreateTariffCommand(Guid TariffId) : ICommand;