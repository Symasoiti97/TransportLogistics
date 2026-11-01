using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Tariff view model
/// </summary>
public sealed class TariffView
{
    /// <summary>
    /// Tariff identifier
    /// </summary>
    /// <example>d8aae288-7ae9-4536-bc69-00e74cc85865</example>
    [NotDefault]
    public Guid Id { get; set; }

    /// <summary>
    /// Route
    /// </summary>
    public RouteView? Route { get; set; }

    /// <summary>
    /// Cargo
    /// </summary>
    public CargoType? CargoType { get; set; }

    /// <summary>
    /// Container ownership
    /// </summary>
    public ContainerOwn? ContainerOwn { get; set; }

    /// <summary>
    /// Container size
    /// </summary>
    public ContainerSize? ContainerSize { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public PriceView? Price { get; set; }

    /// <summary>
    /// Profile identifier of the tariff's current manager
    /// </summary>
    /// <example>5f1b7781-fc42-4b02-8e4d-b105de4ca85f</example>
    [NotDefault]
    public Guid ManagerProfileId { get; set; }

    /// <summary>
    /// Indicates whether the tariff is a draft or active
    /// True - the tariff is a draft
    /// False - the tariff is active
    /// </summary>
    /// <example>true</example>
    [Required]
    public bool IsDraft { get; set; }
}
