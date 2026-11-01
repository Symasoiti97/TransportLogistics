using System.ComponentModel.DataAnnotations;
using TL.SharedKernel.Business.Aggregates.Enums;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Request to save a tariff with price parameters
/// </summary>
public sealed class SaveTariffPriceRequest
{
    /// <summary>
    /// Price
    /// </summary>
    /// <example>251.2</example>
    [Required]
    public decimal Price { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    /// <example>USD</example>
    [NotDefault]
    public CurrencyCode CurrencyCode { get; set; }
}
