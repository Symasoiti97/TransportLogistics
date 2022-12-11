using System.ComponentModel.DataAnnotations;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение тарифа с параметрами цены
/// </summary>
public sealed class SaveTariffPriceRequest
{
    /// <summary>
    /// Цена
    /// </summary>
    /// <example>251.2</example>
    [Required]
    public decimal Price { get; set; }

    /// <summary>
    /// Код валюты
    /// </summary>
    /// <example>USD</example>
    //[Required]
    public string CurrencyCode { get; set; } = null!;
}