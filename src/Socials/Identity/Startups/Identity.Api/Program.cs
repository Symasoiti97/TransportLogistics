using Hellang.Middleware.ProblemDetails;
using ProblemDetailsOptions = Hellang.Middleware.ProblemDetails.ProblemDetailsOptions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails((Action<ProblemDetailsOptions>?) null);
builder.Services.AddHttpLogging();

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