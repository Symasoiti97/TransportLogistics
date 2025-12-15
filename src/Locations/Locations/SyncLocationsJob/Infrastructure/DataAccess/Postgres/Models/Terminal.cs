using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

public sealed record Terminal(
    long OsmId,
    string? OsmType,
    string? Name,
    string? NameEn,
    string? NameRu,
    string? Building,
    string? Amenity,
    Dictionary<string, string>? Tags,
    Geometry? Geom,
    Point? Centroid);
