using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Events;

/// <inheritdoc />
public sealed class TariffCreated(Guid eventId, Guid tariffId) : Event(eventId)
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    public Guid TariffId { get; } = tariffId;

    /// <summary>
    /// Создает событие об создании тарифа
    /// </summary>
    /// <param name="tariffId">Идентификатор тарифа</param>
    /// <returns>Событие</returns>
    public static TariffCreated Create(Guid tariffId) => new(Guid.NewGuid(), tariffId);
}