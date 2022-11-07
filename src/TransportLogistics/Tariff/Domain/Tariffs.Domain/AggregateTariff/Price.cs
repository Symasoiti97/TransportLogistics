using Domain.Abstracts;

namespace Tariffs.Domain.AggregateTariff;

/// <summary>
/// Цена
/// </summary>
public sealed class Price : ValueObject
{
    /// <summary>
    /// Значение
    /// </summary>
    public decimal Value { get; private set; }

    /// <summary>
    /// Код валюты
    /// </summary>
    public string CurrencyCode { get; private set; } = null!;

    public Price(decimal value, string currencyCode)
    {
        SetValue(value);
        SetCurrencyCode(currencyCode);
    }

    private void SetValue(decimal value)
    {
        Value = value;
    }

    private void SetCurrencyCode(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Value can't be empty", nameof(currencyCode));

        CurrencyCode = currencyCode;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new object[] {Value, CurrencyCode};
    }
}