using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

public class Way
{
    public long Id { get; set; }
    public int Version { get; set; }
    public int UserId { get; set; }
    public DateTime Tstamp { get; set; }
    public long ChangesetId { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public long[] Nodes { get; set; }
    public Geometry Bbox { get; set; }
    public Geometry Linestring { get; set; }
}