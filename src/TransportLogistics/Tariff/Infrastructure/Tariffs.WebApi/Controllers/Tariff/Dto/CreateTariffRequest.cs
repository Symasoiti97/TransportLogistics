using System.ComponentModel.DataAnnotations;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Запрос на создание тарифа
/// </summary>
public class CreateTariffRequest
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    /// <example>7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4</example>
    [NotDefault]
    public Guid TariffId { get; set; }
}