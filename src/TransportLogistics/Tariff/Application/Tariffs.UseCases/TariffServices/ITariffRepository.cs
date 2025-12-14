using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

public interface ITariffRepository
{
    Task<Tariff> GetAsync(Guid tariffId, CancellationToken cancellationToken);

    Task AddAsync(Tariff tariff, CancellationToken cancellationToken);

    Task UpdateAsync(Tariff tariff, CancellationToken cancellationToken);
}
