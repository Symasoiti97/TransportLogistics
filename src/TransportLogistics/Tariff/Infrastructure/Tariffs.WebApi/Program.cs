using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.AspNet.Extensions;
using TL.SharedKernel.Infrastructure.AspNet.Options;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection;
using TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;
using ProblemDetailsExtensions =
    TL.SharedKernel.Infrastructure.AspNet.Extensions.ProblemDetailsExtensions;
using SwaggerGenOptionsExtensions =
    TL.SharedKernel.Infrastructure.AspNet.Extensions.SwaggerGenOptionsExtensions;

var errorTypes
    = new[]
        {
            typeof(AssemblyReference).Assembly,
            typeof(TL.TransportLogistics.Tariffs.Business.Aggregates.AssemblyReference).Assembly
        }
        .SelectMany(assembly => assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Error))))
        .ToArray();

var builder = WebApplication.CreateBuilder(args);

var apiOptions = builder.Configuration.GetRequiredSectionValue<ApiOptions>("ApiOptions");
builder.Services.AddSingleton(apiOptions);

builder.Services
    .AddHttpLogging(_ => { })
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = {info => ErrorJsonTypeInfoModifier.Modify(info, errorTypes)}
        };
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddSingleton(provider =>
    provider.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => SwaggerGenOptionsExtensions.SwaggerGenOptionsAction(
    new ServiceSwaggerGenOptions(
        options,
        apiOptions.Name,
        errorTypes)));

builder.Services.AddTariffServices(builder.Configuration.GetRequiredSectionValue<Neo4JSettings>("Neo4jSettings"));

builder.Services.AddLocalization();

builder.Services.AddProblemDetails(ProblemDetailsExtensions.Configure);

builder.Services.Configure<ApiBehaviorOptions>(ApiBehaviorOptionsExtensions.Configure);

var app = builder.Build();

app.UseProblemDetails();

app.UseHttpLogging();

var serviceSettings = app.Services.GetRequiredService<ApiOptions>();
app.UsePathBase($"/{serviceSettings.Name}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            $"{apiOptions.Name}/swagger.json",
            apiOptions.Name);
        options.SwaggerEndpoint(
            $"{apiOptions.Name + "-errors"}/swagger.json",
            apiOptions.Name + "-errors");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();