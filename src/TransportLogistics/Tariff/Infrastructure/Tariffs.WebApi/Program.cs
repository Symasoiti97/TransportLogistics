using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;
using ProblemDetailsExtensions =
    TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions.ProblemDetailsExtensions;
using SwaggerGenOptionsExtensions =
    TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions.SwaggerGenOptionsExtensions;

var errorTypes
    = new[]
        {
            typeof(TL.SharedKernel.Business.Aggregates.AssemblyReference).Assembly,
            typeof(TL.TransportLogistics.Tariffs.Business.Aggregates.AssemblyReference).Assembly
        }
        .SelectMany(assembly => assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Error))))
        .ToArray();

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Configuration.GetRequiredSectionValue<ApiSettings>("ApiSettings");
builder.Services.AddSingleton(settings);

builder.Services
    .AddHttpLogging(_ => { })
    .AddControllers()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = {info => ErrorJsonTypeInfoModifier.Modify(info, errorTypes)}
            };
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

builder.Services.AddSingleton(
    provider => provider.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options => SwaggerGenOptionsExtensions.SwaggerGenOptionsAction(
        new ServiceSwaggerGenOptions(
            options,
            settings.Name,
            errorTypes)));

builder.Services.AddTariffServices(builder.Configuration.GetRequiredSectionValue<Neo4JSettings>("Neo4jSettings"));

builder.Services.AddLocalization();

builder.Services.AddProblemDetails(ProblemDetailsExtensions.Configure);

builder.Services.Configure<ApiBehaviorOptions>(ApiBehaviorOptionsExtensions.Configure);

var app = builder.Build();

app.UseProblemDetails();

app.UseHttpLogging();

var serviceSettings = app.Services.GetRequiredService<ApiSettings>();
app.UsePathBase($"/{serviceSettings.Name}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint(
                $"{settings.Name}/swagger.json",
                settings.Name);
            options.SwaggerEndpoint(
                $"{settings.Name + "-errors"}/swagger.json",
                settings.Name + "-errors");
        });
}

app.UseAuthorization();

app.MapControllers();

app.Run();