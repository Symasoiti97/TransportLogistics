using System.ComponentModel.DataAnnotations;
using TL.SharedKernel.Business.Aggregates.Enums;

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
    [NotDefault]
    public CurrencyCode CurrencyCode { get; set; }
}
