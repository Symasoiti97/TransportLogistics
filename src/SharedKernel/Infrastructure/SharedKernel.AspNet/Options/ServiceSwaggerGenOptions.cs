using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.SharedKernel.Infrastructure.AspNet.Options;

public sealed class ServiceSwaggerGenOptions
{
    public SwaggerGenOptions SwaggerGenOptions { get; }
    public string DocumentName { get; }
    public Type[] ErrorTypes { get; }

    public ServiceSwaggerGenOptions(SwaggerGenOptions swaggerGenOptions, string documentName, Type[] errorTypes)
    {
        SwaggerGenOptions = swaggerGenOptions;
        DocumentName = documentName;
        ErrorTypes = errorTypes;
    }
}