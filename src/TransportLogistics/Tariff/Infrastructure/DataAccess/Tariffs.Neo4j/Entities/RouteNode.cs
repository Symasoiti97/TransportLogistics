using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

internal sealed class RouteNode
{
    public string Hash { get; init; } = null!;
    public RouteType Type { get; init; }
}