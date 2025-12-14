using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

public sealed record SaveTariffRouteCommand(Guid TariffId, Route Route) : IUseCase;
