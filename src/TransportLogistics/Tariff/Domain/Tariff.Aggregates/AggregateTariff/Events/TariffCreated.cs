using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Events;

/// <inheritdoc />
public sealed class TariffCreated(Guid eventId, Guid tariffId) : Event(eventId)
{
    public Guid TariffId { get; } = tariffId;

    public static TariffCreated Create(Guid tariffId) => new(Guid.NewGuid(), tariffId);
}
