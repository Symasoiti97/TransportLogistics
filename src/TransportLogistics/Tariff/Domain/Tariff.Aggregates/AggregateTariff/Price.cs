using System.ComponentModel.Exceptions;
using TL.SharedKernel.Business.Aggregates.Enums;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

public sealed record Price
{
    public decimal Value { get; private set; }
    public CurrencyCode CurrencyCode { get; private set; }

    public Price(decimal value, CurrencyCode currencyCode)
    {
        InvalidEnumArgumentException.ThrowIfUndefined(currencyCode);

        Value = value;
        CurrencyCode = currencyCode;
    }
}
