using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

internal sealed class LocationNode
{
    public Guid Id { get; set; }
    public LocationType Type { get; set; }
}