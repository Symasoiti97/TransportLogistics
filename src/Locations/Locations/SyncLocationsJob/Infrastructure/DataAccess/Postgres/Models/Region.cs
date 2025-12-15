using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

public sealed record Region(
    long OsmId,
    string? Name,
    string? NameEn,
    string? NameRu,
    int? AdminLevel,
    string? PostalCode,
    long? Population,
    Dictionary<string, string>? Tags,
    Geometry? Geom,
    Point? Centroid);
