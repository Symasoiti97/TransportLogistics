using System.Text.Json;
using Hellang.Middleware.ProblemDetails;
using TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;
using TL.Socials.Identity.Infrastructure.DependencyInjection;
using TL.Socials.Identity.Infrastructure.Services.Options;
using TL.Socials.Identity.Startups.Api.BackgroundServices;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
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