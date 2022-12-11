using TL.SharedKernel.Business.Aggregates;

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
    public Price(decimal value, string currencyCode)
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
    public string CurrencyCode { get; private set; } = null!;

    private void SetValue(decimal value)
    {
        Value = value;
    }

    private void SetCurrencyCode(string currencyCode)
    {
        Error.Throw().IfNullOrWhiteSpace(currencyCode);

        CurrencyCode = currencyCode;
    }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new object[] {Value, CurrencyCode};
    }
}