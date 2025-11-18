using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Hellang.Middleware.ProblemDetails;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.AspNet.Extensions;
using TL.SharedKernel.Infrastructure.AspNet.Options;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;
using TL.Socials.Identity.Infrastructure.DependencyInjection;
using TL.Socials.Identity.Infrastructure.Services.Options;
using TL.Socials.Identity.Startups.Api.BackgroundServices;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;
using SwaggerGenOptionsExtensions = TL.SharedKernel.Infrastructure.AspNet.Extensions.SwaggerGenOptionsExtensions;

var errorTypes
    = new[]
        {
            typeof(AssemblyReference).Assembly
        }
        .SelectMany(assembly => assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Error))))
        .ToArray();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var apiOptions = configuration.GetRequiredSectionValue<ApiOptions>("ApiOptions");
builder.Services.AddSingleton(apiOptions);

builder.Services.AddControllers()
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

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => SwaggerGenOptionsExtensions.SwaggerGenOptionsAction(
    new ServiceSwaggerGenOptions(
        options,
        apiOptions.Name,
        errorTypes)));

builder.Services.AddProblemDetails((Action<ProblemDetailsOptions>?) null);
builder.Services.AddHttpLogging();
builder.Services.AddIdentityServices(
    configuration.GetRequiredSectionValue<JwtTokenOptions>("JwtTokenOptions"),
    configuration.GetRequiredConnectionString("IdentityPostgres"),
    configuration.GetRequiredConnectionString("IdentityRedis"),
    builder.Environment.IsDevelopment());
builder.Services.AddSingleton(
    configuration.GetRequiredSectionValue<EventProcessorOptions>("RedisEventProcessorOptions"));
builder.Services.AddTransient<EventProcessor>();
builder.Services.AddHostedService<EventProcessWorker>();

builder.Services.AddSingleton(_ =>
{
    var resolver = new EventTypeInfoResolver();
    return new JsonSerializerOptions
    {
        WriteIndented = true,
        TypeInfoResolver = resolver
    };
});

var app = builder.Build();

app.UseProblemDetails();

app.UseHttpLogging();

app.UsePathBase($"/{configuration["ServiceName"]}");

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