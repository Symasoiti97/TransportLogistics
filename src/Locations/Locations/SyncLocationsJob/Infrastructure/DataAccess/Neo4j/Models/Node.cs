using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

public sealed class Node
{
    public long Id { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public Point? Geom { get; set; }
}