using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

public sealed record Country(
    long OsmId,
    string? Name,
    string? NameEn,
    string? NameRu,
    string? Iso3166_1,
    string? Iso3166_1Alpha2,
    string? Iso3166_1Alpha3,
    int? AdminLevel,
    long? Population,
    Dictionary<string, string>? Tags,
    Geometry? Geom,
    Point? Centroid);
