-- osm2pgsql Flex output configuration for location data
-- This configuration creates separate tables for different location types
-- matching the LocationType enum in the C# code

local tables = {}

-- Countries table (admin_level=2)
tables.countries = osm2pgsql.define_table({
    name = 'countries',
    ids = { type = 'relation', id_column = 'osm_id' },
    columns = {
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'iso3166_1', type = 'text', not_null = false },
        { column = 'iso3166_1_alpha2', type = 'text', not_null = false },
        { column = 'iso3166_1_alpha3', type = 'text', not_null = false },
        { column = 'admin_level', type = 'int', not_null = false },
        { column = 'population', type = 'bigint', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'geometry', not_null = false },
        { column = 'centroid', type = 'point', not_null = false }
    }
})

-- Regions table (admin_level varies by country)
tables.regions = osm2pgsql.define_table({
    name = 'regions',
    ids = { type = 'relation', id_column = 'osm_id' },
    columns = {
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'admin_level', type = 'int', not_null = false },
        { column = 'postal_code', type = 'text', not_null = false },
        { column = 'population', type = 'bigint', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'geometry', not_null = false },
        { column = 'centroid', type = 'point', not_null = false }
    }
})

-- Cities table
tables.cities = osm2pgsql.define_table({
    name = 'cities',
    ids = { type = 'any', id_column = 'osm_id' },
    columns = {
        { column = 'osm_type', type = 'text', not_null = false },
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'place', type = 'text', not_null = false },
        { column = 'postal_code', type = 'text', not_null = false },
        { column = 'population', type = 'bigint', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'geometry', not_null = false },
        { column = 'centroid', type = 'point', not_null = false }
    }
})

-- Railway stations table
tables.railways = osm2pgsql.define_table({
    name = 'railways',
    ids = { type = 'any', id_column = 'osm_id' },
    columns = {
        { column = 'osm_type', type = 'text', not_null = false },
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'railway', type = 'text', not_null = false },
        { column = 'station', type = 'text', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'point', not_null = false }
    }
})

-- Ports table
tables.ports = osm2pgsql.define_table({
    name = 'ports',
    ids = { type = 'any', id_column = 'osm_id' },
    columns = {
        { column = 'osm_type', type = 'text', not_null = false },
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'harbour', type = 'text', not_null = false },
        { column = 'seamark_type', type = 'text', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'geometry', not_null = false },
        { column = 'centroid', type = 'point', not_null = false }
    }
})

-- Terminals table
tables.terminals = osm2pgsql.define_table({
    name = 'terminals',
    ids = { type = 'any', id_column = 'osm_id' },
    columns = {
        { column = 'osm_type', type = 'text', not_null = false },
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'building', type = 'text', not_null = false },
        { column = 'amenity', type = 'text', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'geometry', not_null = false },
        { column = 'centroid', type = 'point', not_null = false }
    }
})

-- Warehouses table
tables.warehouses = osm2pgsql.define_table({
    name = 'warehouses',
    ids = { type = 'any', id_column = 'osm_id' },
    columns = {
        { column = 'osm_type', type = 'text', not_null = false },
        { column = 'name', type = 'text', not_null = false },
        { column = 'name_en', type = 'text', not_null = false },
        { column = 'name_ru', type = 'text', not_null = false },
        { column = 'building', type = 'text', not_null = false },
        { column = 'industrial', type = 'text', not_null = false },
        { column = 'tags', type = 'jsonb', not_null = false },
        { column = 'geom', type = 'geometry', not_null = false },
        { column = 'centroid', type = 'point', not_null = false }
    }
})

-- Helper function to get multilingual names
function get_ml_names(tags)
    return {
        name = tags.name,
        name_en = tags['name:en'] or tags['name:en-US'] or tags['int_name'],
        name_ru = tags['name:ru']
    }
end

-- Helper function to convert tags to JSON
function tags_to_json(tags)
    local json_tags = {}
    for k, v in pairs(tags) do
        json_tags[k] = v
    end
    return json_tags
end

-- Process relations (countries and regions)
function osm2pgsql.process_relation(object)
    if object.tags.boundary == 'administrative' then
        local admin_level = tonumber(object.tags.admin_level)
        if not admin_level then return end
        
        local names = get_ml_names(object.tags)
        if not names.name then return end
        
        local geom = object:as_multipolygon()
        if not geom then return end
        
        -- Countries (admin_level = 2)
        if admin_level == 2 and object.tags['ISO3166-1'] then
            tables.countries:insert({
                name = names.name,
                name_en = names.name_en,
                name_ru = names.name_ru,
                iso3166_1 = object.tags['ISO3166-1'],
                iso3166_1_alpha2 = object.tags['ISO3166-1:alpha2'],
                iso3166_1_alpha3 = object.tags['ISO3166-1:alpha3'],
                admin_level = admin_level,
                population = tonumber(object.tags.population),
                tags = tags_to_json(object.tags),
                geom = geom,
                centroid = geom:centroid()
            })
        -- Regions (admin_level 3-8)
        elseif admin_level >= 3 and admin_level <= 8 then
            tables.regions:insert({
                name = names.name,
                name_en = names.name_en,
                name_ru = names.name_ru,
                admin_level = admin_level,
                postal_code = object.tags['addr:postcode'] or object.tags.postal_code,
                population = tonumber(object.tags.population),
                tags = tags_to_json(object.tags),
                geom = geom,
                centroid = geom:centroid()
            })
        end
    end
end

-- Process ways
function osm2pgsql.process_way(object)
    local names = get_ml_names(object.tags)
    
    -- Cities (from ways - less common but possible)
    if object.tags.place and (
        object.tags.place == 'city' or 
        object.tags.place == 'town' or 
        object.tags.place == 'village' or 
        object.tags.place == 'hamlet'
    ) and names.name then
        local geom = object:as_polygon() or object:as_linestring()
        if geom then
            tables.cities:insert({
                osm_type = 'way',
                name = names.name,
                name_en = names.name_en,
                name_ru = names.name_ru,
                place = object.tags.place,
                postal_code = object.tags['addr:postcode'] or object.tags.postal_code,
                population = tonumber(object.tags.population),
                tags = tags_to_json(object.tags),
                geom = geom,
                centroid = geom:centroid()
            })
        end
    end
    
    -- Ports (ways)
    if (object.tags.harbour == 'yes' or 
        object.tags['seamark:type'] == 'harbour' or 
        object.tags.landuse == 'port' or
        object.tags.landuse == 'harbour') and names.name then
        local geom = object:as_polygon() or object:as_linestring()
        if geom then
            tables.ports:insert({
                osm_type = 'way',
                name = names.name,
                name_en = names.name_en,
                name_ru = names.name_ru,
                harbour = object.tags.harbour,
                seamark_type = object.tags['seamark:type'],
                tags = tags_to_json(object.tags),
                geom = geom,
                centroid = geom:centroid()
            })
        end
    end
    
    -- Terminals (ways)
    if (object.tags.building == 'terminal' or 
        object.tags.amenity == 'ferry_terminal') and names.name then
        local geom = object:as_polygon() or object:as_linestring()
        if geom then
            tables.terminals:insert({
                osm_type = 'way',
                name = names.name,
                name_en = names.name_en,
                name_ru = names.name_ru,
                building = object.tags.building,
                amenity = object.tags.amenity,
                tags = tags_to_json(object.tags),
                geom = geom,
                centroid = geom:centroid()
            })
        end
    end
    
    -- Warehouses (ways)
    if (object.tags.building == 'warehouse' or 
        object.tags.industrial == 'warehouse') and names.name then
        local geom = object:as_polygon() or object:as_linestring()
        if geom then
            tables.warehouses:insert({
                osm_type = 'way',
                name = names.name,
                name_en = names.name_en,
                name_ru = names.name_ru,
                building = object.tags.building,
                industrial = object.tags.industrial,
                tags = tags_to_json(object.tags),
                geom = geom,
                centroid = geom:centroid()
            })
        end
    end
end

-- Process nodes
function osm2pgsql.process_node(object)
    local names = get_ml_names(object.tags)
    local geom = object:as_point()
    
    -- Cities
    if object.tags.place and (
        object.tags.place == 'city' or 
        object.tags.place == 'town' or 
        object.tags.place == 'village' or 
        object.tags.place == 'hamlet'
    ) and names.name then
        tables.cities:insert({
            osm_type = 'node',
            name = names.name,
            name_en = names.name_en,
            name_ru = names.name_ru,
            place = object.tags.place,
            postal_code = object.tags['addr:postcode'] or object.tags.postal_code,
            population = tonumber(object.tags.population),
            tags = tags_to_json(object.tags),
            geom = geom,
            centroid = geom
        })
    end
    
    -- Railway stations
    if (object.tags.railway == 'station' or object.tags.railway == 'halt') and
       object.tags.station ~= 'subway' and names.name then
        -- Skip if it's explicitly marked as not a train station
        if object.tags.train == 'no' or object.tags.transport == 'subway' then
            return
        end
        
        tables.railways:insert({
            osm_type = 'node',
            name = names.name,
            name_en = names.name_en,
            name_ru = names.name_ru,
            railway = object.tags.railway,
            station = object.tags.station,
            tags = tags_to_json(object.tags),
            geom = geom
        })
    end
    
    -- Ports (nodes)
    if (object.tags.harbour == 'yes' or 
        object.tags['seamark:type'] == 'harbour' or
        object.tags.landuse == 'port') and names.name then
        tables.ports:insert({
            osm_type = 'node',
            name = names.name,
            name_en = names.name_en,
            name_ru = names.name_ru,
            harbour = object.tags.harbour,
            seamark_type = object.tags['seamark:type'],
            tags = tags_to_json(object.tags),
            geom = geom,
            centroid = geom
        })
    end
    
    -- Terminals (nodes)
    if (object.tags.building == 'terminal' or 
        object.tags.amenity == 'ferry_terminal') and names.name then
        tables.terminals:insert({
            osm_type = 'node',
            name = names.name,
            name_en = names.name_en,
            name_ru = names.name_ru,
            building = object.tags.building,
            amenity = object.tags.amenity,
            tags = tags_to_json(object.tags),
            geom = geom,
            centroid = geom
        })
    end
    
    -- Warehouses (nodes)
    if (object.tags.building == 'warehouse' or 
        object.tags.industrial == 'warehouse') and names.name then
        tables.warehouses:insert({
            osm_type = 'node',
            name = names.name,
            name_en = names.name_en,
            name_ru = names.name_ru,
            building = object.tags.building,
            industrial = object.tags.industrial,
            tags = tags_to_json(object.tags),
            geom = geom,
            centroid = geom
        })
    end
end