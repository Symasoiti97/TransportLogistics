using Microsoft.EntityFrameworkCore;
using Npgsql;
using TL.Locations.Locations.SyncLocationsTool;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.Services;
using TL.Locations.Locations.SyncLocationsTool.Settings;
using TL.SharedKernel.Infrastructure.AspNet.Extensions;
using TL.SharedKernel.Infrastructure.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddNeo4JServices(
            context.Configuration.GetRequiredSectionValue<Neo4JSettings>("Neo4JSettings"));

        // Configure Npgsql data source with dynamic JSON and NetTopologySuite enabled
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            context.Configuration.GetConnectionString("OsmPostgres"));
        dataSourceBuilder.EnableDynamicJson();
        dataSourceBuilder.UseNetTopologySuite();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<OsmDbContext>(builder =>
            builder.UseNpgsql(dataSource, o => o.UseNetTopologySuite())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddTransient<SyncLocationsService>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
