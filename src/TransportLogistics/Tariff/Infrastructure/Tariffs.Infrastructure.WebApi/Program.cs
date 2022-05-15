using Tariffs.Application.DependencyInjection;
using Tariffs.Infrastructure.WebApi.Controllers.Tariff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(expression => { expression.AddProfile<TariffMapperProfile>(); });
builder.Services.AddTariffServices();

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