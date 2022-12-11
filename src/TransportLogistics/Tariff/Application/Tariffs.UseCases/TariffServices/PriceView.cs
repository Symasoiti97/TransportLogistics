using System.ComponentModel.DataAnnotations;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Цена
/// </summary>
public sealed class PriceView
{
    /// <summary>
    /// Значение
    /// </summary>
    /// <example>325.51</example>
    [Required]
    public decimal Value { get; set; }

    /// <summary>
    /// Кода валюты
    /// </summary>
    /// <example>USD</example>
    [Required(AllowEmptyStrings = false)]
    public string CurrencyCode { get; set; } = null!;
}