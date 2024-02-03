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
    public string MultiLanguageName { get; set; }
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
/// Тип локации
/// </summary>
public enum LocationType
{
    /// <summary>
    /// Мир
    /// </summary>
    World = 1,

    /// <summary>
    /// Страна
    /// </summary>
    Country = 2,

    /// <summary>
    /// Регион
    /// </summary>
    Region = 3,

    /// <summary>
    /// Город
    /// </summary>
    City = 4,

    /// <summary>
    /// Порт
    /// </summary>
    Port = 5,

    /// <summary>
    /// Железнодоржная станция
    /// </summary>
    Railway = 6,

    /// <summary>
    /// Терминал, может пренадлежать, как порту, так и ж/д станции
    /// </summary>
    Terminal = 7,

    /// <summary>
    /// Склад
    /// </summary>
    Warehouse = 8
}