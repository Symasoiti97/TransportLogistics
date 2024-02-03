namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

public class RelationMember
{
    public long RelationId { get; set; }
    public long MemberId { get; set; }
    public char MemberType { get; set; }
    public string MemberRole { get; set; }
    public int SequenceId { get; set; }
}