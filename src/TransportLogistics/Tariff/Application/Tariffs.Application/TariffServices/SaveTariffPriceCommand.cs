using Application.Abstracts;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Команда сохранения тарифа с параметрами цены
/// </summary>
public class SaveTariffPriceCommand : ICommand
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    public Guid TariffId { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Валюта
    /// </summary>
    public string CurrencyCode { get; set; } = null!;
}