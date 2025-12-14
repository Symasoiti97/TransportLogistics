using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

public class TariffNotFound : NotFound
{
    public Guid TariffId { get; }

    /// <inheritdoc />
    public override string Message => "Tariff not found.";

    public TariffNotFound(Guid tariffId)
    {
        TariffId = tariffId;
    }
}
