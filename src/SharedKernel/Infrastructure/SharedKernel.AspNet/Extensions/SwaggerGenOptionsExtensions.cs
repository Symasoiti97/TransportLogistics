using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TL.SharedKernel.Infrastructure.AspNet.Options;
using TL.SharedKernel.Infrastructure.Swagger;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions;

public static class SwaggerGenOptionsExtensions
{
    public static void SwaggerGenOptionsAction(ServiceSwaggerGenOptions options)
    {
        var errorsDocumentName = options.DocumentName + "-errors";
        options.SwaggerGenOptions.SwaggerDoc(
            options.DocumentName,
            new OpenApiInfo
            {
                Title = options.DocumentName,
                Version = "v1"
            });
        options.SwaggerGenOptions.SwaggerDoc(
            errorsDocumentName,
            new OpenApiInfo
            {
                Title = errorsDocumentName,
                Version = "v1"
            });

        options.SwaggerGenOptions.SupportNonNullableReferenceTypes();
        options.SwaggerGenOptions.UseAllOfToExtendReferenceSchemas();
        options.SwaggerGenOptions.DocInclusionPredicate((docName, _) => docName == options.DocumentName);

        options.SwaggerGenOptions.CustomSchemaIds(type => type.BuildSwaggerSchemaName());

        foreach (var fileName in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
        {
            options.SwaggerGenOptions.IncludeXmlComments(fileName, includeControllerXmlComments: true);
        }

        options.SwaggerGenOptions.SchemaFilter<EnumSchemaFilter>(AppContext.BaseDirectory);
        options.SwaggerGenOptions.SchemaFilter<AnnotationOperationSchemaFilter>();
        options.SwaggerGenOptions.DocumentFilter<RegisterErrorSchemesDocumentFilter>(
            errorsDocumentName,
            options.ErrorTypes);

        options.SwaggerGenOptions.DocumentFilter<ProblemDetailsSchemaDocumentFilter>();
    }
}