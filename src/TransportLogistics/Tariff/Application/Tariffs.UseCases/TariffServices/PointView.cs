using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Route point
/// </summary>
public sealed class PointView
{
    /// <summary>
    /// Location
    /// </summary>
    /// <example>dfefe432-a273-4ff7-bacd-c9701fe8afcf</example>
    [NotDefault]
    public Guid LocationId { get; set; }

    /// <summary>
    /// Point type
    /// </summary>
    /// <example>Fob</example>
    [Required]
    public PointType Type { get; set; }

    /// <summary>
    /// Sequence number
    /// </summary>
    /// <example>1</example>
    [MinLength(1)]
    public ushort Order { get; set; }

    /// <summary>
    /// Unique point hash
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Hash { get; set; } = null!;
}
