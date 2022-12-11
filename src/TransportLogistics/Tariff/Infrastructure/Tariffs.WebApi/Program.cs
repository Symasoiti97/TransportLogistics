using System.Text.Json;
using System.Text.Json.Serialization;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Filters;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;
using ProblemDetailsExtensions = TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions.ProblemDetailsExtensions;
using SwaggerExtensions = TL.TransportLogistics.Tariffs.Startups.WebApi.Extensions.SwaggerGenOptionsExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(GetServiceSettings(builder.Configuration));

builder.Services
    .AddControllers(
        options => { options.Filters.Add<ValidateModelFilterAttribute>(); })
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new SubTypeConverter<Error>());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

builder.Services.AddSingleton(
    provider => provider.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(SwaggerExtensions.SwaggerGenOptionsAction);

builder.Services.AddTariffServices(GetNeo4JSettings(builder.Configuration));

builder.Services.AddLocalization();

builder.Services.AddProblemDetails(ProblemDetailsExtensions.Configure);

var app = builder.Build();

app.UseProblemDetails();

var serviceSettings = app.Services.GetRequiredService<ServiceSettings>();
app.UsePathBase($"/{serviceSettings.Name}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint(
                $"{SwaggerExtensions.TariffApiDocumentName}/swagger.json",
                SwaggerExtensions.TariffApiInfo.Title);
            options.SwaggerEndpoint(
                $"{SwaggerExtensions.TariffApiErrorsDocumentName}/swagger.json",
                SwaggerExtensions.TariffApiErrorsInfo.Title);
        });
}

app.UseHttpLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();

static Neo4JSettings GetNeo4JSettings(IConfiguration configuration)
{
    return new Neo4JSettings(
        new Uri(configuration["Neo4jSettings:Uri"]),
        configuration["Neo4jSettings:UserName"],
        configuration["Neo4jSettings:Password"]);
}

static ServiceSettings GetServiceSettings(IConfiguration configuration)
{
    return new ServiceSettings(
        configuration["ServiceName"]);
}