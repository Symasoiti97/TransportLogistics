using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

internal sealed class TariffNode
{
    public Guid Id { get; init; }
    public Guid ManagerProfileId { get; init; }
    public bool IsDraft { get; init; }
    public decimal? Price { get; init; }
    public string? CurrencyCode { get; set; }
    public CargoType? CargoType { get; init; }
    public ContainerOwn? ContainerOwn { get; init; }
    public ContainerSize? ContainerSize { get; init; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}