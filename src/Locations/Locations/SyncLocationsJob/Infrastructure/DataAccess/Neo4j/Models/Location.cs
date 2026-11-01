namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

public sealed class Location
{
    public Guid Id { get; set; }
    public long SyncId { get; set; }
    public string? Ru { get; set; }
    public string? En { get; set; }
    public string? Origin { get; set; }
    public LocationType Type { get; set; }
    public LocationSourceType SourceType { get; set; }
    public long Population { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? MultiLanguageName { get; set; }
    public string? Code { get; set; }
    public string? PostalCode { get; set; }
}

public enum LocationSourceType
{
    /// <summary>
    /// From system (For example - developer)
    /// </summary>
    System = 1,

    /// <summary>
    /// Locations from OpenStreetMap
    /// </summary>
    OsmNode = 2,

    /// <summary>
    /// Locations from OpenStreetMap
    /// </summary>
    OsmRelation = 3
}

/// <summary>
/// Location type
/// </summary>
public enum LocationType
{
    /// <summary>
    /// World
    /// </summary>
    World = 1,

    /// <summary>
    /// Country
    /// </summary>
    Country = 2,

    /// <summary>
    /// Region
    /// </summary>
    Region = 3,

    /// <summary>
    /// City
    /// </summary>
    City = 4,

    /// <summary>
    /// Port
    /// </summary>
    Port = 5,

    /// <summary>
    /// Railway station
    /// </summary>
    Railway = 6,

    /// <summary>
    /// Terminal; can belong to either a port or a railway station
    /// </summary>
    Terminal = 7,

    /// <summary>
    /// Warehouse
    /// </summary>
    Warehouse = 8
}
