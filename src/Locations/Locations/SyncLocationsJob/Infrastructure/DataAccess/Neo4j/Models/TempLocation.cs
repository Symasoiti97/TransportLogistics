namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

public class TempLocation
{
    public long Id { get; set; }
    public Dictionary<string, string> Tags { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public long ParentNodeId { get; set; }
    public long Population { get; set; }
    public string ParentName { get; set; }
}