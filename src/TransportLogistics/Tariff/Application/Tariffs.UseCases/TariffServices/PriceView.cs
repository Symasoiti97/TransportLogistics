using System.ComponentModel.DataAnnotations;
using TL.SharedKernel.Business.Aggregates.Enums;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Price
/// </summary>
public sealed class PriceView
{
    /// <summary>
    /// Value
    /// </summary>
    /// <example>325.51</example>
    [Required]
    public decimal Value { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    /// <example>USD</example>
    [NotDefault]
    public CurrencyCode CurrencyCode { get; set; }
}
