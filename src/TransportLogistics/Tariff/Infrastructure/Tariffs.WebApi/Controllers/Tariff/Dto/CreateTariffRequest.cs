using System.ComponentModel.DataAnnotations;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Request to create a tariff
/// </summary>
public sealed class CreateTariffRequest
{
    /// <summary>
    /// Tariff identifier
    /// </summary>
    /// <example>7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4</example>
    [NotDefault]
    public Guid TariffId { get; set; }
}
