using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

public sealed record City(
    long OsmId,
    string? OsmType,
    string? Name,
    string? NameEn,
    string? NameRu,
    string? Place,
    string? PostalCode,
    long? Population,
    Dictionary<string, string>? Tags,
    Geometry? Geom,
    Point? Centroid);
