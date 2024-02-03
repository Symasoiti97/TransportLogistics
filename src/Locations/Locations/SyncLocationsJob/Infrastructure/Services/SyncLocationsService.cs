using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres;
using TL.SharedKernel.Infrastructure.Neo4j;
using UnidecodeSharpFork;
using Location = TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models.Location;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.Services;

/// <summary>
/// Импортер локаций из postgres(OpenStreetMap) в Neo4j
/// </summary>
/// <remarks>
/// Импорт локаций происходит вниз по иерархии (Мир -> Страны -> Регионы -> Города -> Станции ж/д)
/// У каждой локации(Кроме Мир) должен быть родитель. Если у локации нет родителя, то ее нужно удалить
/// </remarks>
internal class SyncLocationsService
{
    private readonly OsmDbContext _osmDbContext;
    private readonly ICypherGraphClientFactory _graphClientFactory;

    //excludeCountries - страны дубликаты(11980, 1252792, 9604462)
    private static readonly long[] ExcludeCountries = {11980, 1252792, 9604462};
    private static readonly long[] IncludeCountries = {59065};
    private static readonly long[] IncludeRegions = {59189};
    private static readonly long[] IgnoreRegionCountries = {2202162, 2978650};

    private static readonly long[] ExcludeRegions =
        {1692123, 9581354, 72639, 3795586, 3788485, 1574364, 3082668, 9604138, 11819141, 11871225, 12101417};

    public SyncLocationsService(OsmDbContext osmDbContext, ICypherGraphClientFactory graphClientFactory)
    {
        _osmDbContext = osmDbContext ?? throw new ArgumentNullException(nameof(osmDbContext));
        _graphClientFactory = graphClientFactory ?? throw new ArgumentNullException(nameof(graphClientFactory));
    }

    public async Task Sync()
    {
        await _osmDbContext.Database.ExecuteSqlRawAsync("SET enable_seqscan = OFF;");

        var worldLocation = await SaveWorldLocationAsync();
        foreach (var country in await GetAndSaveCountriesAsync(worldLocation))
        {
            if (!IgnoreRegionCountries.Contains(country.SyncId))
            {
                var regions = await GetRegionsAsync(country).ToListAsync();

                var regionsWithoutExcludeRegions = regions.Where(x => !ExcludeRegions.Contains(x.SyncId));
                await SaveLocationsAsync(regionsWithoutExcludeRegions, country);

                foreach (var region in regionsWithoutExcludeRegions)
                {
                    await ImportCitiesAndRailways(region);
                }

                var cities = await GetCitiesAsync(country.SyncId, regions.Select(x => x.SyncId)).ToArrayAsync();
                await SaveLocationsAsync(cities, country);

                foreach (var city in cities)
                {
                    var railways = await GetRailwaysAsync(city).ToArrayAsync();
                    await SaveLocationsAsync(railways, city);
                }
            }
            else
            {
                await ImportCitiesAndRailways(country);
            }
        }

        async Task ImportCitiesAndRailways(Location parentLocation)
        {
            try
            {
                var cities = await GetCities(parentLocation.SyncId).ToArrayAsync();
                await SaveLocationsAsync(cities, parentLocation);

                try
                {
                    var railwaysWithCity = GetRailwaysAsync(cities.Select(x => x.SyncId));
                    await foreach (var (city, railways) in railwaysWithCity)
                    {
                        try
                        {
                            await SaveLocationsAsync(await railways.ToArrayAsync(), city);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            Console.WriteLine(JsonSerializer.Serialize(city));
                        }
                    }
                }
                catch
                {
                    foreach (var city in cities)
                    {
                        var railways = (await GetRailwaysAsync(new[] {city.SyncId}).FirstOrDefaultAsync()).Item2;
                        try
                        {
                            await SaveLocationsAsync(await railways.ToArrayAsync(), city);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                            Console.WriteLine(JsonSerializer.Serialize(city));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.WriteLine(JsonSerializer.Serialize(parentLocation));
            }
        }
    }

    private async Task<IReadOnlyCollection<Location>> GetAndSaveCountriesAsync(Location worldLocation)
    {
        var countries = await GetCountries().Where(x => !ExcludeCountries.Contains(x.SyncId)).ToListAsync();
        await SaveLocationsAsync(countries, worldLocation);
        return countries;
    }

    private IAsyncEnumerable<Location> GetCountries()
    {
        var result = _osmDbContext.Set<Node>().FromSqlInterpolated(
            $"""
             SELECT r.id "Id", COALESCE(n.tags, r.tags) "Tags", n.geom "Geom"
             FROM relations r
                      LEFT JOIN relation_members rm on r.id = rm.relation_id AND member_role = 'label'
                      LEFT JOIN nodes n on rm.member_id = n.id
             WHERE (r.tags -> 'admin_level' = '2' AND r.tags -> 'boundary' = 'administrative' AND r.tags-> 'ISO3166-1' is not null) OR r.id = 7750160;
             """);

        return result.AsAsyncEnumerable().Select(
            x => new Location
            {
                Id = Guid.NewGuid(),
                Type = LocationType.Country,
                Origin = x.Tags.GetValueOrDefault("name"),
                En = x.Tags.TryGetValue("name:en", out var nameEn) ? nameEn :
                    x.Tags.TryGetValue("name", out nameEn) && nameEn.IsBasicLatinFirstSymbol() ? nameEn :
                    nameEn.Unidecode(),
                Ru = x.Tags.TryGetValue("name:ru", out var nameRu) ? nameRu :
                    x.Tags.TryGetValue("name", out nameRu) && nameRu.IsCyrillicFirstSymbol() ? nameRu :
                    nameRu.Unidecode(),
                Population =
                    x.Tags.TryGetValue("population", out var population) &&
                    long.TryParse(population, out var populationNumber)
                        ? populationNumber
                        : 0,
                MultiLanguageName = x.Tags.FilterNamesByCultures().BuildFullTxt(),
                SyncId = x.Id,
                SourceType = LocationSourceType.OsmRelation,
                Code = x.Tags.TryGetValue("ISO3166-1", out var code) ? code : null,
                Latitude = x.Geom?.Y,
                Longitude = x.Geom?.X
            });
    }

    private IAsyncEnumerable<Location> GetRegionsAsync(Location parentLocation)
    {
        var result = _osmDbContext.Set<TempLocation>().FromSqlInterpolated(
            $"""
             SELECT region.id "Id", region.tags "Tags", st_y(st_centroid(st_polygonize(w.linestring))) "Latitude", st_x(st_centroid(st_polygonize(w.linestring))) "Longitude"
             FROM (SELECT region.*
                   FROM relations region
             
             INNER JOIN relation_members rm on region.id = rm.member_id
             INNER JOIN relations r on rm.relation_id = r.id
             
             INNER JOIN relation_members rm2 on r.id = rm2.member_id
             INNER JOIN relations r2 on rm2.relation_id = r2.id
             
             INNER JOIN relation_members rm3 on r2.id = rm3.member_id
             INNER JOIN relations r3 on rm3.relation_id = r3.id AND r3.id = {parentLocation.SyncId}
             INNER JOIN region_admin_levels c on r3.tags -> 'ISO3166-1' = c."Code" AND region.tags -> 'admin_level' = ANY(c."Levels")
                 WHERE region.tags -> 'name' is not null
                   AND c."Levels" != '{Array.Empty<string>()}'
             UNION ALL
             SELECT region.*
             FROM relations region
             INNER JOIN relation_members rm on region.id = rm.member_id
             INNER JOIN relations r on rm.relation_id = r.id
             
             INNER JOIN relation_members rm2 on r.id = rm2.member_id
             INNER JOIN relations r2 on rm2.relation_id = r2.id AND r2.id = {parentLocation.SyncId}
             INNER JOIN region_admin_levels c on r2.tags -> 'ISO3166-1' = c."Code" AND region.tags -> 'admin_level' = ANY(c."Levels")
                                            WHERE region.tags -> 'name' is not null
                                                AND c."Levels" != '{Array.Empty<string>()}'
             UNION ALL
             SELECT region.*
             FROM relations region
             INNER JOIN relation_members rm on region.id = rm.member_id
             INNER JOIN relations r on rm.relation_id = r.id AND r.id = {parentLocation.SyncId}
             INNER JOIN region_admin_levels c on r.tags -> 'ISO3166-1' = c."Code" AND region.tags -> 'admin_level' = ANY(c."Levels")
                                            WHERE region.tags -> 'name' is not null AND c."Levels" != '{Array.Empty<string>()}'
                                            ) region
             INNER JOIN relation_members rm on region.id = rm.relation_id AND rm.member_role = 'outer'
             INNER JOIN ways w on w.id = rm.member_id
             GROUP BY region.id, region.tags ORDER BY region.tags -> 'admin_level';
             """);

        return result.AsAsyncEnumerable().Select(
            x => new Location
            {
                Id = Guid.NewGuid(),
                Type = LocationType.Region,
                Origin = x.Tags.GetValueOrDefault("name"),
                En = x.Tags.TryGetValue("name:en", out var nameEn) ? nameEn :
                    x.Tags.TryGetValue("name", out nameEn) && nameEn.IsBasicLatinFirstSymbol() ? nameEn :
                    nameEn.Unidecode(),
                Ru = x.Tags.TryGetValue("name:ru", out var nameRu) ? nameRu :
                    x.Tags.TryGetValue("name", out nameRu) && nameRu.IsCyrillicFirstSymbol() ? nameRu :
                    nameRu.Unidecode(),
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Population = x.Population,
                PostalCode = x.Tags.TryGetValue("addr:postcode", out var postcode)
                    ? postcode
                    : x.Tags.GetValueOrDefault("postal_code"),
                MultiLanguageName = x.Tags.FilterNamesByCultures().BuildFullTxt(),
                SyncId = x.Id,
                SourceType = LocationSourceType.OsmRelation
            });
    }

    private IAsyncEnumerable<Location> GetCities(long relationId)
    {
        var query = _osmDbContext.Set<Node>().FromSqlInterpolated(
            $"""
             SELECT city.* FROM nodes city
             WHERE city.tags -> 'name' is not null AND (city.tags -> 'place' = 'city' OR city.tags -> 'place' = 'town' OR city.tags -> 'place' = 'village' OR city.tags -> 'place' = 'hamlet')
               AND st_contains((SELECT CASE WHEN COUNT(*) = 1 THEN first(p.polygon) ELSE st_difference(last(p.polygon), first(p.polygon)) END polygon
                                FROM (SELECT st_polygonize(ST_ForceClosed(st_linemerge(p.polygon))) polygon
                                      FROM (SELECT st_union(w.linestring) polygon, rm.member_role as role
                                            FROM relations r
                                                     INNER JOIN relation_members rm on r.id = rm.relation_id AND (rm.member_role = 'outer' OR rm.member_role = 'inner')
                                                     INNER JOIN ways w on w.id = rm.member_id
                                            WHERE r.id = {relationId}
                                            GROUP BY rm.member_role) p
                                      GROUP BY role) p), city.geom)
             """);

        return query.AsAsyncEnumerable().Select(
            x => new Location
            {
                Id = Guid.NewGuid(),
                Origin = x.Tags.GetValueOrDefault("name"),
                En = x.Tags.TryGetValue("name:en", out var nameEn) ? nameEn :
                    x.Tags.TryGetValue("name", out nameEn) && nameEn.IsBasicLatinFirstSymbol() ? nameEn :
                    nameEn.Unidecode(),
                Ru = x.Tags.TryGetValue("name:ru", out var nameRu) ? nameRu :
                    x.Tags.TryGetValue("name", out nameRu) && nameRu.IsCyrillicFirstSymbol() ? nameRu :
                    nameRu.Unidecode(),
                Population =
                    x.Tags.TryGetValue("population", out var population) &&
                    long.TryParse(population, out var populationNumber)
                        ? populationNumber
                        : 0,
                Latitude = x.Geom.Y,
                Longitude = x.Geom.X,
                Type = LocationType.City,
                SyncId = x.Id,
                SourceType = LocationSourceType.OsmNode,
                PostalCode = x.Tags.TryGetValue("addr:postcode", out var postcode)
                    ? postcode
                    : x.Tags.GetValueOrDefault("postal_code"),
                MultiLanguageName = x.Tags.FilterNamesByCultures().BuildFullTxt()
            });
    }

    private IAsyncEnumerable<Location> GetCitiesAsync(long countrySyncId, IEnumerable<long> regionsSyncIds)
    {
        var query = _osmDbContext.Set<Node>().FromSqlInterpolated(
            $"""
             SELECT city.*
             FROM nodes city
             WHERE city.tags -> 'name' is not null AND
                                    (city.tags -> 'place' = 'city' OR city.tags -> 'place' = 'town' OR city.tags -> 'place' = 'village' OR city.tags -> 'place' = 'hamlet') AND
                                    st_contains((SELECT st_difference(p1.polygon, p2.polygon) polygon
                                  FROM (SELECT st_polygonize(w.linestring) as polygon
                                        FROM relations r
                                                 INNER JOIN relation_members rm on r.id = rm.relation_id AND (rm.member_role = 'outer' OR rm.member_role = 'inner')
                                                 INNER JOIN ways w on w.id = rm.member_id
                                        WHERE r.id = {countrySyncId}) p1
                                           INNER JOIN (SELECT st_polygonize(p.polygon) as polygon
                                                       FROM (SELECT st_union(w.linestring) as polygon
                                                             FROM relations r
                                                                      INNER JOIN relation_members rm on r.id = rm.relation_id AND (rm.member_role = 'outer' OR rm.member_role = 'inner')
                                                                      INNER JOIN ways w on w.id = rm.member_id
                                                             WHERE r.id IN ({string.Join(", ", regionsSyncIds)})
                                                             GROUP BY r.id, rm.member_role) p) p2 ON true), city.geom)
             """);

        return query.AsAsyncEnumerable().Select(
            x => new Location
            {
                Id = Guid.NewGuid(),
                Origin = x.Tags.GetValueOrDefault("name"),
                En = x.Tags.TryGetValue("name:en", out var nameEn) ? nameEn :
                    x.Tags.TryGetValue("name", out nameEn) && nameEn.IsBasicLatinFirstSymbol() ? nameEn :
                    nameEn.Unidecode(),
                Ru = x.Tags.TryGetValue("name:ru", out var nameRu) ? nameRu :
                    x.Tags.TryGetValue("name", out nameRu) && nameRu.IsCyrillicFirstSymbol() ? nameRu :
                    nameRu.Unidecode(),
                Population =
                    x.Tags.TryGetValue("population", out var population) &&
                    long.TryParse(population, out var populationNumber)
                        ? populationNumber
                        : 0,
                Latitude = x.Geom.Y,
                Longitude = x.Geom.X,
                Type = LocationType.City,
                SyncId = x.Id,
                SourceType = LocationSourceType.OsmNode,
                PostalCode = x.Tags.TryGetValue("addr:postcode", out var postcode)
                    ? postcode
                    : x.Tags.GetValueOrDefault("postal_code"),
                MultiLanguageName = x.Tags.FilterNamesByCultures().BuildFullTxt()
            });
    }

    private IAsyncEnumerable<Location> GetRailwaysAsync(Location parentLocation)
    {
        var result = _osmDbContext.Set<TempLocation>().FromSqlInterpolated(
            $"""
             SELECT railway.id, railway.tags, st_y(railway.geom), st_x(railway.geom)
             FROM nodes railway
             WHERE ((railway.tags -> 'railway' = 'station' AND (railway.tags -> 'station' is null OR railway.tags -> 'station' <> 'subway' OR railway.tags -> 'train' = 'yes')) OR (railway.tags -> 'railway' = 'halt')) AND railway.tags -> 'name' is not null
               AND st_contains((SELECT polygon
                                FROM (SELECT st_polygonize(r2.linestring) polygon, n.geom geom, (city.tags -> 'admin_level')::integer admin_level
                                      FROM relations city
                                               INNER JOIN nodes n on lower(trim(n.tags -> 'name')) = lower(trim(city.tags -> 'name'))
                                               INNER JOIN relation_members rm on city.id = rm.relation_id AND rm.member_role = 'outer'
                                               INNER JOIN ways r2 on r2.id = rm.member_id
                                      WHERE n.id = {parentLocation.SyncId}
                                      GROUP BY n.id, admin_level ORDER BY admin_level DESC LIMIT 1) parent
                                WHERE st_contains(polygon, geom)), railway.geom);
             """);

        return result.AsAsyncEnumerable()
            .Where(x => !x.Tags.TryGetValue("transport", out var transport) || transport == "train").Select(
                x => new Location
                {
                    Id = Guid.NewGuid(),
                    Origin = x.Tags.GetValueOrDefault("name"),
                    En = x.Tags.TryGetValue("name:en", out var nameEn) ? nameEn :
                        x.Tags.TryGetValue("name", out nameEn) && nameEn.IsBasicLatinFirstSymbol() ? nameEn :
                        nameEn.Unidecode(),
                    Ru = x.Tags.TryGetValue("name:ru", out var nameRu) ? nameRu :
                        x.Tags.TryGetValue("name", out nameRu) && nameRu.IsCyrillicFirstSymbol() ? nameRu :
                        nameRu.Unidecode(),
                    Population = x.Population,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    Type = LocationType.Railway,
                    SyncId = x.Id,
                    SourceType = LocationSourceType.OsmNode,
                    MultiLanguageName = x.Tags.FilterNamesByCultures().BuildFullTxt()
                });
    }

    private IAsyncEnumerable<(Location, IAsyncEnumerable<Location>)> GetRailwaysAsync(IEnumerable<long> nodeIds)
    {
        var result = _osmDbContext.Set<TempLocation>().FromSqlInterpolated(
            $"""
             SELECT railway.id, railway.tags, st_y(railway.geom), st_x(railway.geom), parent.id, parent.tags
             FROM nodes railway
                      INNER JOIN (SELECT first(parent.polygon) polygon, first(parent.id) id, first(parent.tags) tags FROM (SELECT st_polygonize(ST_ForceClosed(st_linemerge(n.polygon))) polygon, n.id, n.tags, n.geom
                                  FROM (SELECT st_union(r2.linestring) polygon, n.id id, n.tags tags, n.geom geom, (city.tags -> 'admin_level')::int as admin_level
                                        FROM relations city
                                                 INNER JOIN nodes n on n.tags -> 'name' = city.tags -> 'name'
                                                 INNER JOIN relation_members rm on city.id = rm.relation_id AND rm.member_role = 'outer'
                                                 INNER JOIN ways r2 on r2.id = rm.member_id
                                        WHERE n.id in ({string.Join(", ", nodeIds)}) AND city.tags -> 'ISO3166-1' is null
                                        GROUP BY city.id, n.id ORDER BY admin_level DESC) n
                                  GROUP BY n.id, n.tags, n.id, n.geom, polygon) parent WHERE st_contains(parent.polygon, parent.geom)) parent
                                 on (parent.polygon is not null AND
                                    (railway.tags -> 'railway' = 'station' AND (railway.tags -> 'station' is null OR railway.tags -> 'station' <> 'subway') OR (railway.tags -> 'railway' = 'halt')) AND
                                    railway.tags -> 'name' is not null AND st_contains(parent.polygon, railway.geom));
             """
        );

        return result.AsAsyncEnumerable().GroupBy(
            x => new {x.ParentNodeId, x.ParentName},
            (parent, nodes) =>
                new ValueTuple<Location, IAsyncEnumerable<Location>>(
                    new Location
                    {
                        Id = Guid.NewGuid(),
                        Type = LocationType.City,
                        SyncId = parent.ParentNodeId,
                        SourceType = LocationSourceType.OsmNode,
                        En = parent.ParentName,
                        Ru = parent.ParentName,
                        Origin = parent.ParentName
                    },
                    nodes.Where(x => !x.Tags.TryGetValue("transport", out var transport) || transport == "train")
                        .Select(
                            x => new Location
                            {
                                Id = Guid.NewGuid(),
                                Origin = x.Tags.GetValueOrDefault("name"),
                                En = x.Tags.TryGetValue("name:en", out var nameEn)
                                    ? nameEn
                                    : x.Tags.TryGetValue("name", out nameEn) && nameEn.IsBasicLatinFirstSymbol()
                                        ? nameEn
                                        : nameEn.Unidecode(),
                                Ru = x.Tags.TryGetValue("name:ru", out var nameRu)
                                    ? nameRu
                                    : x.Tags.TryGetValue("name", out nameRu) && nameRu.IsCyrillicFirstSymbol()
                                        ? nameRu
                                        : nameRu.Unidecode(),
                                Population = x.Population,
                                Latitude = x.Latitude,
                                Longitude = x.Longitude,
                                Type = LocationType.Railway,
                                SyncId = x.Id,
                                SourceType = LocationSourceType.OsmNode,
                                MultiLanguageName = x.Tags.FilterNamesByCultures().BuildFullTxt()
                            })));
    }

    private async Task<(Location location, IEnumerable<KeyValuePair<string, string>> names)[]> GetRailwaysAsync(
        long nodeId,
        IEnumerable<KeyValuePair<string, string>> names)
    {
        var query = await _osmDbContext.Set<Node>().FromSqlInterpolated(
                $"""
                 SELECT railway.*
                 FROM nodes railway
                          INNER JOIN (SELECT st_polygonize(array_agg(r2.linestring)) as polygon
                                      FROM relations city
                                               INNER JOIN nodes n on n.tags -> 'name' = city.tags -> 'name' AND
                                                                     n.tags -> 'place' = city.tags -> 'place' AND
                                                                     ((n.tags -> 'addr:country' is null OR city.tags -> 'addr:country' is null) OR
                                                                      (n.tags -> 'addr:country' is not null AND city.tags -> 'addr:country' is not null AND n.tags -> 'addr:country' = city.tags -> 'addr:country')) AND
                                                                     ((n.tags -> 'addr:region' is null OR city.tags -> 'addr:region' is null) OR
                                                                      (n.tags -> 'addr:region' is not null AND city.tags -> 'addr:region' is not null AND n.tags -> 'addr:region' = city.tags -> 'addr:region'))
                                               INNER JOIN relation_members rm on city.id = rm.relation_id AND rm.member_role = 'outer'
                                               INNER JOIN ways r2 on r2.id = rm.member_id
                                      WHERE n.id = {nodeId}) parent
                                     on railway.tags -> 'railway' = 'station' AND st_contains(parent.polygon, railway.geom)
                 """)
            .ToArrayAsync();

        return query.Select(
            x => new ValueTuple<Location, IEnumerable<KeyValuePair<string, string>>>(
                new Location
                {
                    Id = Guid.NewGuid(),
                    En = x.Tags.TryGetValue("name:en", out var nameEn) ? nameEn :
                        x.Tags.TryGetValue("name", out nameEn) ? nameEn : null,
                    Ru = x.Tags.TryGetValue("name:ru", out var nameRu) ? nameRu :
                        x.Tags.TryGetValue("name", out nameRu) ? nameRu : null,
                    Latitude = x.Geom.X,
                    Longitude = x.Geom.Y,
                    Type = LocationType.Railway,
                    SyncId = x.Id,
                    SourceType = LocationSourceType.OsmNode,
                    MultiLanguageName = x.Tags.FilterNamesByCultures().JoinNames(names).BuildFullTxt()
                },
                x.Tags.FilterNamesByCultures().JoinNames(names))).ToArray();
    }

    private async Task SaveLocationsAsync(IEnumerable<Location> locations, Location hLocation)
    {
        Console.Write(
            $"{DateTime.UtcNow}: [START] where parentName = {hLocation.Ru}\tparentLocation = {hLocation.SyncId}\tSourceType: {hLocation.SourceType}\tType: {hLocation.Type}\t");

        const int batchSize = 1000;
        var count = 0;
        var locationBatch = new List<Location>(batchSize);
        foreach (var location in locations)
        {
            count++;
            locationBatch.Add(location);
            if (locationBatch.Count == batchSize)
            {
                await InnerSaveLocations();
                locationBatch.Clear();
            }
        }

        if (locationBatch.Count < batchSize)
        {
            await InnerSaveLocations();
        }

        Console.WriteLine($"[END]: {DateTime.UtcNow}\tCount: {count}");

        async Task InnerSaveLocations()
        {
            var query = (await _graphClientFactory.GetTransactionCypherGraphClientAsync(CancellationToken.None))
                .Cypher
                .Unwind(locationBatch, "newL")
                .Match("(h:Location {SyncId: {parentSyncId}, Type: {parentType}, SourceType: {parentSourceType}})")
                .Merge("(l:Location {SyncId: newL.SyncId, Type: newL.Type, SourceType: newL.SourceType})")
                .With("l, h, l.Id as id, newL")
                .OptionalMatch("(l)-[r:LOCATED_IN]->(:Location)")
                .Delete("r")
                .Merge("(l)-[:LOCATED_IN]->(h)")
                .Set(
                    "l = newL, l.Id = CASE WHEN id is null THEN newL.Id ELSE id END, l.WeightType = size((l)-[:LOCATED_IN*]->())")
                .WithParams(
                    new
                    {
                        parentSyncId = hLocation.SyncId,
                        IAsyncEnumerableparentType = hLocation.Type,
                        parentSourceType = hLocation.SourceType
                    });

            await query.ExecuteWithoutResultsAsync();
        }
    }

    private async Task<Location> SaveWorldLocationAsync()
    {
        var worldLocation = new Location
        {
            Id = Guid.NewGuid(),
            SyncId = 0,
            Ru = "Мир",
            En = "World",
            SourceType = LocationSourceType.System,
            Type = LocationType.World
        };
        var query = (await _graphClientFactory.GetTransactionCypherGraphClientAsync(CancellationToken.None)).Cypher
            .Unwind(new[] {worldLocation}, "newL")
            .Merge("(l:NewLocation {SyncId: newL.SyncId, Type: newL.Type, SourceType: newL.SourceType})")
            .With("l, l.Id as id, newL")
            .Set("l = newL, l.Id = CASE WHEN id is null THEN newL.Id ELSE id END");

        await query.ExecuteWithoutResultsAsync();

        return worldLocation;
    }
}