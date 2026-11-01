using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Command to publish a tariff
/// Moves a tariff from draft to active (creates a copy)
/// </summary>
public sealed record PublishTariffCommand(Guid TariffId) : IUseCase;
