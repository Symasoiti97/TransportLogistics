using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

public sealed record GetTariffQuery(Guid TariffId) : IUseCase<TariffView>;
