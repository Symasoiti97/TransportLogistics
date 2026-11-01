using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Request to save a tariff with cargo parameters
/// </summary>
public sealed class SaveTariffCargoRequest
{
    /// <summary>
    /// Container ownership
    /// </summary>
    [Required]
    public ContainerOwn ContainerOwn { get; set; }

    /// <summary>
    /// Cargo type
    /// </summary>
    [Required]
    public CargoType CargoType { get; set; }

    /// <summary>
    /// Container size
    /// </summary>
    [Required]
    public ContainerSize ContainerSize { get; set; }
}
