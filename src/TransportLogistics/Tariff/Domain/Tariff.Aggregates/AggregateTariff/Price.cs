using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Business.Aggregates.Enums;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Цена
/// </summary>
public sealed class Price : ValueObject
{
    /// <summary>
    /// Cоздать <see cref="Price"/>
    /// </summary>
    /// <param name="value">Цена</param>
    /// <param name="currencyCode">Код валюты</param>
    public Price(decimal value, CurrencyCode currencyCode)
    {
        SetValue(value);
        SetCurrencyCode(currencyCode);
    }

    /// <summary>
    /// Значение
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Код валюты
    /// </summary>
    public CurrencyCode CurrencyCode { get; private set; }

    private void SetValue(decimal value)
    {
        Value = value;
    }

    private void SetCurrencyCode(CurrencyCode currencyCode)
    {
        if (!Enum.IsDefined(currencyCode))
        {
            throw new ArgumentException("Currency code must be defined.", nameof(currencyCode));
        }

        CurrencyCode = currencyCode;
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new object[] {Value, CurrencyCode};
    }
}