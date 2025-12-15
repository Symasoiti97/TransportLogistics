using NetTopologySuite.Geometries;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

public sealed record Railway(
    long OsmId,
    string? OsmType,
    string? Name,
    string? NameEn,
    string? NameRu,
    string? RailwayType,
    string? Station,
    Dictionary<string, string>? Tags,
    Point? Geom);
