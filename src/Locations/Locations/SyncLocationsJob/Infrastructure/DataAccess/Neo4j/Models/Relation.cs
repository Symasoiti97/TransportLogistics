namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

public class Relation
{
    public long Id { get; set; }
    public int Version { get; set; }
    public int UserId { get; set; }
    public DateTime Tstamp { get; set; }
    public long ChangesetId { get; set; }
    public Dictionary<string, string> Tags { get; set; }
}