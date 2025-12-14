using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

public sealed record CreateTariffCommand(Guid TariffId, Guid ManagerProfileId) : IUseCase;
