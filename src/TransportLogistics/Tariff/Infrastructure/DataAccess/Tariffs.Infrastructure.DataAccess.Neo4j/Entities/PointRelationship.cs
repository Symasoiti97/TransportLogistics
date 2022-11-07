using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

internal sealed class PointRelationship
{
    public string Hash { get; init; } = null!;
    public PointType Type { get; init; }
    public int Order { get; init; }
}