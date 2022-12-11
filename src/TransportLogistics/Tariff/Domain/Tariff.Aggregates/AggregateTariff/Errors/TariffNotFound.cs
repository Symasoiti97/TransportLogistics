using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

/// <summary>
/// Тариф не найден
/// </summary>
public class TariffNotFound : NotFound
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    public Guid TariffId { get; }

    /// <inheritdoc />
    public override string Message => "Tariff not found.";

    /// <summary>
    /// Создать <see cref="TariffNotFound"/>
    /// </summary>
    /// <param name="tariffId">Идентификатор тарифа</param>
    public TariffNotFound(Guid tariffId) : base($"Not found tariff with id '{tariffId}'")
    {
        TariffId = tariffId;
    }
}