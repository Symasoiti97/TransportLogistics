using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Hellang.Middleware.ProblemDetails;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;
using TL.Socials.Identity.Infrastructure.DependencyInjection;
using TL.Socials.Identity.Infrastructure.Services.Options;
using TL.Socials.Identity.Startups.Api.BackgroundServices;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

var errorTypes
    = new[]
        {
            typeof(AssemblyReference).Assembly
        }
        .SelectMany(assembly => assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Error))))
        .ToArray();

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers()
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
;

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

builder.Services.AddSingleton(
    _ =>
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
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Identity API v1"); });
}

app.UseAuthorization();

app.MapControllers();

app.Run();