using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

public sealed record Port(
    long OsmId,
    string? OsmType,
    string? Name,
    string? NameEn,
    string? NameRu,
    string? Harbour,
    string? SeamarkType,
    Dictionary<string, string>? Tags,
    Geometry? Geom,
    Point? Centroid);
