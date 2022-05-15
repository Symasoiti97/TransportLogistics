namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на сохранение тарифа с параметрами цены
/// </summary>
public class SaveTariffPriceRequest
{
    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Код валюты
    /// </summary>
    public string CurrencyCode { get; set; } = null!;
}