using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;
using TL.SharedKernel.Infrastructure.Neo4j;
using UnidecodeSharpFork;
using Location = TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models.Location;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.Services;

/// <summary>
/// Импортер локаций из postgres(OpenStreetMap) в Neo4j
/// </summary>
/// <remarks>
/// Импорт локаций происходит вниз по иерархии (Мир -> Страны -> Регионы -> Города -> Станции ж/д/Порты -> Терминалы/Склады)
/// У каждой локации(Кроме Мир) должен быть родитель. Если у локации нет родителя, то ее нужно удалить
/// </remarks>
internal sealed class SyncLocationsService
{
    //excludeCountries - страны дубликаты(11980, 1252792, 9604462)
    private static readonly long[] ExcludeCountries = [11980, 1252792, 9604462];
    private static readonly long[] IncludeCountries = [59065];
    private static readonly long[] IncludeRegions = [59189];
    private static readonly long[] IgnoreRegionCountries = [2202162, 2978650];

    private static readonly long[] ExcludeRegions =
        [1692123, 9581354, 72639, 3795586, 3788485, 1574364, 3082668, 9604138, 11819141, 11871225, 12101417];

    private readonly ICypherGraphClientFactory _graphClientFactory;
    private readonly OsmDbContext _osmDbContext;

    public SyncLocationsService(OsmDbContext osmDbContext, ICypherGraphClientFactory graphClientFactory)
    {
        _osmDbContext = osmDbContext ?? throw new ArgumentNullException(nameof(osmDbContext));
        _graphClientFactory = graphClientFactory ?? throw new ArgumentNullException(nameof(graphClientFactory));
    }

    public async Task SyncAsync()
    {
        await _osmDbContext.Database.ExecuteSqlRawAsync("SET enable_seqscan = OFF;");

        var worldLocation = await SaveWorldLocationAsync();
        foreach (var country in await GetAndSaveCountriesAsync(worldLocation))
        {
            if (!IgnoreRegionCountries.Contains(country.SyncId))
            {
                var regions = await GetRegionsInCountryAsync(country.SyncId);
                var regionsWithoutExcluded = regions.Where(region => !ExcludeRegions.Contains(region.SyncId)).ToList();
                await SaveLocationsAsync(regionsWithoutExcluded, country);

                foreach (var region in regionsWithoutExcluded)
                {
                    await ImportCitiesAndSublocations(region);

                    var portsInRegion = await GetPortsInRegionAsync(region.SyncId);
                    await SaveLocationsAsync(portsInRegion, region);
                    await ImportTerminalsForPorts(portsInRegion);
                }
            }
            else
            {
                await ImportCitiesAndSublocations(country);
            }

            var portsInCountry = await GetPortsInCountryAsync(country.SyncId);
            await SaveLocationsAsync(portsInCountry, country);
            await ImportTerminalsForPorts(portsInCountry);
        }

        async Task ImportCitiesAndSublocations(Location parentLocation)
        {
            try
            {
                var cities = await GetCitiesInParentAsync(parentLocation);
                await SaveLocationsAsync(cities, parentLocation);

                foreach (var city in cities)
                {
                    var railways = await GetRailwaysInCityAsync(city.SyncId);
                    await SaveLocationsAsync(railways, city);

                    await ImportTerminalsForRailways(railways);

                    var warehouses = await GetWarehousesInCityAsync(city.SyncId);
                    await SaveLocationsAsync(warehouses, city);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Console.WriteLine(JsonSerializer.Serialize(parentLocation));
            }
        }

        async Task ImportTerminalsForPorts(IEnumerable<Location> ports)
        {
            foreach (var port in ports)
            {
                var terminals = await GetTerminalsNearLocationAsync(port.SyncId, LocationType.Port);
                await SaveLocationsAsync(terminals, port);
            }
        }

        async Task ImportTerminalsForRailways(IEnumerable<Location> railways)
        {
            foreach (var railway in railways)
            {
                var terminals = await GetTerminalsNearLocationAsync(railway.SyncId, LocationType.Railway);
                await SaveLocationsAsync(terminals, railway);
            }
        }
    }

    private static Location ConvertCountryToLocation(Country country)
        => new()
        {
            Id = Guid.NewGuid(),
            En =
                country.NameEn
                ?? (country.Name?.IsBasicLatinFirstSymbol() == true ? country.Name : country.Name?.Unidecode()),
            Ru =
                country.NameRu
                ?? (country.Name?.IsCyrillicFirstSymbol() == true ? country.Name : country.Name?.Unidecode()),
            Origin = country.Name,
            Population = country.Population ?? 0,
            Latitude = country.Centroid?.Y,
            Longitude = country.Centroid?.X,
            Type = LocationType.Country,
            SyncId = country.OsmId,
            SourceType = LocationSourceType.OsmRelation,
            Code = country.Iso3166_1,
            MultiLanguageName = BuildMultiLanguageName(country.Tags)
        };

    private static Location ConvertRegionToLocation(Region region)
        => new()
        {
            Id = Guid.NewGuid(),
            En = region.NameEn
                 ?? (region.Name?.IsBasicLatinFirstSymbol() == true ? region.Name : region.Name?.Unidecode()),
            Ru = region.NameRu
                 ?? (region.Name?.IsCyrillicFirstSymbol() == true ? region.Name : region.Name?.Unidecode()),
            Origin = region.Name,
            Population = region.Population ?? 0,
            Latitude = region.Centroid?.Y,
            Longitude = region.Centroid?.X,
            Type = LocationType.Region,
            SyncId = region.OsmId,
            SourceType = LocationSourceType.OsmRelation,
            PostalCode = region.PostalCode,
            MultiLanguageName = BuildMultiLanguageName(region.Tags)
        };

    private static Location ConvertCityToLocation(City city)
        => new()
        {
            Id = Guid.NewGuid(),
            En = city.NameEn ?? (city.Name?.IsBasicLatinFirstSymbol() == true ? city.Name : city.Name?.Unidecode()),
            Ru = city.NameRu ?? (city.Name?.IsCyrillicFirstSymbol() == true ? city.Name : city.Name?.Unidecode()),
            Origin = city.Name,
            Population = city.Population ?? 0,
            Latitude = city.Centroid?.Y ?? city.Geom?.Coordinate.Y,
            Longitude = city.Centroid?.X ?? city.Geom?.Coordinate.X,
            Type = LocationType.City,
            SyncId = city.OsmId,
            SourceType = city.OsmType == "node" ? LocationSourceType.OsmNode : LocationSourceType.OsmRelation,
            PostalCode = city.PostalCode,
            MultiLanguageName = BuildMultiLanguageName(city.Tags)
        };

    private static Location ConvertRailwayToLocation(Railway railway)
        => new()
        {
            Id = Guid.NewGuid(),
            En =
                railway.NameEn
                ?? (railway.Name?.IsBasicLatinFirstSymbol() == true ? railway.Name : railway.Name?.Unidecode()),
            Ru = railway.NameRu
                 ?? (railway.Name?.IsCyrillicFirstSymbol() == true ? railway.Name : railway.Name?.Unidecode()),
            Origin = railway.Name,
            Latitude = railway.Geom?.Y,
            Longitude = railway.Geom?.X,
            Type = LocationType.Railway,
            SyncId = railway.OsmId,
            SourceType = LocationSourceType.OsmNode,
            MultiLanguageName = BuildMultiLanguageName(railway.Tags)
        };

    private static Location ConvertPortToLocation(Port port)
        => new()
        {
            Id = Guid.NewGuid(),
            En = port.NameEn ?? (port.Name?.IsBasicLatinFirstSymbol() == true ? port.Name : port.Name?.Unidecode()),
            Ru = port.NameRu ?? (port.Name?.IsCyrillicFirstSymbol() == true ? port.Name : port.Name?.Unidecode()),
            Origin = port.Name,
            Latitude = port.Centroid?.Y ?? port.Geom?.Coordinate.Y,
            Longitude = port.Centroid?.X ?? port.Geom?.Coordinate.X,
            Type = LocationType.Port,
            SyncId = port.OsmId,
            SourceType = port.OsmType == "node" ? LocationSourceType.OsmNode : LocationSourceType.OsmRelation,
            MultiLanguageName = BuildMultiLanguageName(port.Tags)
        };

    private static Location ConvertTerminalToLocation(Terminal terminal)
        => new()
        {
            Id = Guid.NewGuid(),
            En =
                terminal.NameEn
                ?? (terminal.Name?.IsBasicLatinFirstSymbol() == true ? terminal.Name : terminal.Name?.Unidecode()),
            Ru =
                terminal.NameRu
                ?? (terminal.Name?.IsCyrillicFirstSymbol() == true ? terminal.Name : terminal.Name?.Unidecode()),
            Origin = terminal.Name,
            Latitude = terminal.Centroid?.Y ?? terminal.Geom?.Coordinate.Y,
            Longitude = terminal.Centroid?.X ?? terminal.Geom?.Coordinate.X,
            Type = LocationType.Terminal,
            SyncId = terminal.OsmId,
            SourceType = terminal.OsmType == "node" ? LocationSourceType.OsmNode : LocationSourceType.OsmRelation,
            MultiLanguageName = BuildMultiLanguageName(terminal.Tags)
        };

    private static Location ConvertWarehouseToLocation(Warehouse warehouse)
        => new()
        {
            Id = Guid.NewGuid(),
            En =
                warehouse.NameEn
                ?? (warehouse.Name?.IsBasicLatinFirstSymbol() == true ? warehouse.Name : warehouse.Name?.Unidecode()),
            Ru =
                warehouse.NameRu
                ?? (warehouse.Name?.IsCyrillicFirstSymbol() == true ? warehouse.Name : warehouse.Name?.Unidecode()),
            Origin = warehouse.Name,
            Latitude = warehouse.Centroid?.Y ?? warehouse.Geom?.Coordinate.Y,
            Longitude = warehouse.Centroid?.X ?? warehouse.Geom?.Coordinate.X,
            Type = LocationType.Warehouse,
            SyncId = warehouse.OsmId,
            SourceType = warehouse.OsmType == "node" ? LocationSourceType.OsmNode : LocationSourceType.OsmRelation,
            MultiLanguageName = BuildMultiLanguageName(warehouse.Tags)
        };

    private static string? BuildMultiLanguageName(Dictionary<string, string>? tags)
        => tags?.FilterNamesByCultures().BuildFullTxt();

    private async Task<IReadOnlyCollection<Location>> GetAndSaveCountriesAsync(Location worldLocation)
    {
        var countries = await _osmDbContext.Set<Country>()
            .Where(country => !ExcludeCountries.Contains(country.OsmId) || IncludeCountries.Contains(country.OsmId))
            .Select(country => ConvertCountryToLocation(country))
            .ToListAsync();

        await SaveLocationsAsync(countries, worldLocation);
        return countries;
    }

    private async Task<List<Location>> GetRegionsInCountryAsync(long countrySyncId)
    {
        var regions = await _osmDbContext.Set<Region>()
            .FromSqlInterpolated($@"
                SELECT r.* FROM regions r
                WHERE ST_Contains(
                    (SELECT geom FROM countries WHERE osm_id = {countrySyncId}),
                    COALESCE(r.centroid, ST_Centroid(r.geom))
                )
                AND r.admin_level >= 3 AND r.admin_level <= 8
                ORDER BY r.admin_level, r.name")
            .ToListAsync();

        return regions.Select(ConvertRegionToLocation).ToList();
    }

    private async Task<List<Location>> GetCitiesInParentAsync(Location parentLocation)
    {
        var cities = parentLocation.Type == LocationType.Country
            ? await _osmDbContext.Set<City>()
                .FromSqlInterpolated($@"
                    SELECT c.* FROM cities c
                    WHERE ST_Contains(
                        (SELECT geom FROM countries WHERE osm_id = {parentLocation.SyncId}),
                        COALESCE(c.centroid, c.geom)
                    )
                    ORDER BY c.population DESC NULLS LAST, c.name")
                .ToListAsync()
            : await _osmDbContext.Set<City>()
                .FromSqlInterpolated($@"
                    SELECT c.* FROM cities c
                    WHERE ST_Contains(
                        (SELECT geom FROM regions WHERE osm_id = {parentLocation.SyncId}),
                        COALESCE(c.centroid, c.geom)
                    )
                    ORDER BY c.population DESC NULLS LAST, c.name")
                .ToListAsync();

        return cities.Select(ConvertCityToLocation).ToList();
    }

    private async Task<List<Location>> GetRailwaysInCityAsync(long citySyncId)
    {
        var railways = await _osmDbContext.Set<Railway>()
            .FromSqlInterpolated($@"
                SELECT r.* FROM railways r
                WHERE ST_DWithin(
                    r.geom,
                    COALESCE(
                        (SELECT centroid FROM cities WHERE osm_id = {citySyncId} AND centroid IS NOT NULL),
                        (SELECT geom FROM cities WHERE osm_id = {citySyncId})
                    ),
                    0.1  -- ~10km radius
                )
                AND (r.station IS NULL OR r.station != 'subway')
                ORDER BY r.name")
            .ToListAsync();

        return railways.Select(ConvertRailwayToLocation).ToList();
    }

    private async Task<List<Location>> GetPortsInCountryAsync(long countrySyncId)
    {
        var ports = await _osmDbContext.Set<Port>()
            .FromSqlInterpolated($@"
                SELECT p.* FROM ports p
                WHERE ST_Contains(
                    (SELECT geom FROM countries WHERE osm_id = {countrySyncId}),
                    COALESCE(p.centroid, p.geom)
                )
                ORDER BY p.name")
            .ToListAsync();

        return ports.Select(ConvertPortToLocation).ToList();
    }

    private async Task<List<Location>> GetPortsInRegionAsync(long regionSyncId)
    {
        var ports = await _osmDbContext.Set<Port>()
            .FromSqlInterpolated($@"
                SELECT p.* FROM ports p
                WHERE ST_Contains(
                    (SELECT geom FROM regions WHERE osm_id = {regionSyncId}),
                    COALESCE(p.centroid, p.geom)
                )
                ORDER BY p.name")
            .ToListAsync();

        return ports.Select(ConvertPortToLocation).ToList();
    }

    private async Task<List<Location>> GetTerminalsNearLocationAsync(long locationSyncId, LocationType parentType)
    {
        var terminals = parentType == LocationType.Port
            ? await _osmDbContext.Set<Terminal>()
                .FromSqlInterpolated($@"
                    SELECT t.* FROM terminals t
                    WHERE ST_DWithin(
                        COALESCE(t.centroid, t.geom),
                        COALESCE(
                            (SELECT centroid FROM ports WHERE osm_id = {locationSyncId} AND centroid IS NOT NULL),
                            (SELECT geom FROM ports WHERE osm_id = {locationSyncId})
                        ),
                        0.01  -- ~1km radius
                    )
                    ORDER BY t.name")
                .ToListAsync()
            : await _osmDbContext.Set<Terminal>()
                .FromSqlInterpolated($@"
                    SELECT t.* FROM terminals t
                    WHERE ST_DWithin(
                        COALESCE(t.centroid, t.geom),
                        (SELECT geom FROM railways WHERE osm_id = {locationSyncId}),
                        0.01  -- ~1km radius
                    )
                    ORDER BY t.name")
                .ToListAsync();

        return terminals.Select(ConvertTerminalToLocation).ToList();
    }

    private async Task<List<Location>> GetWarehousesInCityAsync(long citySyncId)
    {
        var warehouses = await _osmDbContext.Set<Warehouse>()
            .FromSqlInterpolated($@"
                SELECT w.* FROM warehouses w
                WHERE ST_DWithin(
                    COALESCE(w.centroid, w.geom),
                    COALESCE(
                        (SELECT centroid FROM cities WHERE osm_id = {citySyncId} AND centroid IS NOT NULL),
                        (SELECT geom FROM cities WHERE osm_id = {citySyncId})
                    ),
                    0.05  -- ~5km radius
                )
                ORDER BY w.name")
            .ToListAsync();

        return warehouses.Select(ConvertWarehouseToLocation).ToList();
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
            var query = (await _graphClientFactory.GetCypherFluentQueryAsync(CancellationToken.None))
                .Unwind(locationBatch, "newL")
                .Match(
                    "(h:Location {SyncId: $parentSyncId, Type: $parentType, SourceType: $parentSourceType})")
                .Merge("(l:Location {SyncId: newL.SyncId, Type: newL.Type, SourceType: newL.SourceType})")
                .With("l, h, l.Id as id, newL")
                .OptionalMatch("(l)-[r:LOCATED_IN]->(:Location)")
                .Delete("r")
                .Merge("(l)-[:LOCATED_IN]->(h)")
                .Set(
                    "l = newL, l.Id = CASE WHEN id is null THEN newL.Id ELSE id END, l.WeightType = COUNT {(l)-[:LOCATED_IN*]->()}")
                .WithParams(
                    new
                    {
                        parentSyncId = hLocation.SyncId,
                        parentType = hLocation.Type,
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
        var query = (await _graphClientFactory.GetCypherFluentQueryAsync(CancellationToken.None))
            .Unwind(new[] { worldLocation }, "newL")
            .Merge("(l:Location {SyncId: newL.SyncId, Type: newL.Type, SourceType: newL.SourceType})")
            .With("l, l.Id as id, newL")
            .Set("l = newL, l.Id = CASE WHEN id is null THEN newL.Id ELSE id END");

        await query.ExecuteWithoutResultsAsync();

        return worldLocation;
    }
}
