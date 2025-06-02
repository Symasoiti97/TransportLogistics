using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;

public sealed class ServiceSwaggerGenOptions
{
    public ServiceSwaggerGenOptions(SwaggerGenOptions swaggerGenOptions, string documentName, Type[] errorTypes)
    {
        SwaggerGenOptions = swaggerGenOptions;
        DocumentName = documentName;
        ErrorTypes = errorTypes;
    }

    public SwaggerGenOptions SwaggerGenOptions { get; }
    public string DocumentName { get; }
    public Type[] ErrorTypes { get; }
}