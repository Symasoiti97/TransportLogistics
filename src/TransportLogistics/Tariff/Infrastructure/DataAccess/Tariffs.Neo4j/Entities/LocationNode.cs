using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

internal sealed class LocationNode
{
    public Guid Id { get; set; }
    public LocationType Type { get; set; }
}