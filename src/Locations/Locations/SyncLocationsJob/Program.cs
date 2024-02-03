using Microsoft.EntityFrameworkCore;
using TL.Locations.Locations.SyncLocationsTool;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.Services;
using TL.Locations.Locations.SyncLocationsTool.Settings;
using TL.SharedKernel.Infrastructure.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(
        (context, services) =>
        {
            services.AddNeo4JServices(GetNeo4JSettings(context.Configuration));
            services.AddDbContext<OsmDbContext>(
                builder =>
                    builder.UseNpgsql(
                            context.Configuration.GetConnectionString("OsmPostgres"),
                            optionsBuilder => optionsBuilder.UseNetTopologySuite())
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddTransient<SyncLocationsService>();
            services.AddHostedService<Worker>();
        })
    .Build();


host.Run();

static Neo4JSettings GetNeo4JSettings(IConfiguration configuration)
{
    return new Neo4JSettings(
        new Uri(configuration["Neo4jSettings:Uri"]),
        configuration["Neo4jSettings:UserName"],
        configuration["Neo4jSettings:Password"]);
}