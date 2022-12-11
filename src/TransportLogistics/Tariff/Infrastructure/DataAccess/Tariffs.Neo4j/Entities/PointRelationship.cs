using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

internal sealed class PointRelationship
{
    public string Hash { get; init; } = null!;
    public PointType Type { get; init; }
    public int Order { get; init; }
}