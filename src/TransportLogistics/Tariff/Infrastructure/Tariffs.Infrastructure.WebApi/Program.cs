using System.Text.Json.Serialization;
using Tariffs.Application.DependencyInjection;
using Tariffs.Infrastructure.DependencyInjection;
using Tariffs.Infrastructure.WebApi.Controllers.Tariff;
using Tariffs.Infrastructure.WebApi.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(expression => { expression.AddProfile<TariffMapperProfile>(); });
builder.Services.AddTariffApplicationServices();
builder.Services.AddTariffInfrastructureServices(GetNeo4JSettings(builder.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static Neo4JSettings GetNeo4JSettings(IConfiguration configuration)
{
    return new Neo4JSettings(
        uri: new Uri(configuration["Neo4jSettings:Uri"]),
        userName: configuration["Neo4jSettings:UserName"],
        password: configuration["Neo4jSettings:Password"]);
}