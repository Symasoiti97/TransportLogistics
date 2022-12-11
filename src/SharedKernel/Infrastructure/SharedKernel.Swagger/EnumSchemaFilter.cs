using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TL.SharedKernel.Infrastructure.Swagger;

public sealed class EnumSchemaFilter : ISchemaFilter
{
    private readonly string _documentationDirectoryPath;

    public EnumSchemaFilter(string documentationDirectoryPath)
    {
        _documentationDirectoryPath = documentationDirectoryPath ??
                                      throw new ArgumentNullException(nameof(documentationDirectoryPath));
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Enum is {Count: > 0} && context.Type is {IsEnum: true})
        {
            var xmlCommentFileName = Path.Combine(
                _documentationDirectoryPath,
                context.Type.Assembly.GetName().Name + ".xml");
            if (!File.Exists(xmlCommentFileName))
                return;

            var xPathNavigator = XDocument.Load(xmlCommentFileName).CreateNavigator();

            var htmlSummaryEnums = string.Join(
                string.Empty,
                Enum.GetNames(context.Type)
                    .Select(name => GetSummaryEnum(xPathNavigator, context.Type.FullName!, name))
                    .Where(x => x.Value is not null)
                    .Select(x => $"<li><i>{x.Key}</i> - {x.Value}</li>"));
            if (string.IsNullOrWhiteSpace(htmlSummaryEnums))
                return;

            var description = $"<p><ul>{htmlSummaryEnums}</ul>";
            if (schema.Description == null)
            {
                schema.Description = description;
            }
            else
            {
                schema.Description += description;
            }
        }
    }

    private static KeyValuePair<string, string?> GetSummaryEnum(
        XPathNavigator xPathNavigator,
        string fullTypeName,
        string enumMemberName)
    {
        var fullEnumMemberName = $"F:{fullTypeName}.{enumMemberName}";
        var enumMemberNode = xPathNavigator.SelectSingleNode($"/doc/members/member[@name='{fullEnumMemberName}']");

        var summaryNode = enumMemberNode?.SelectSingleNode("summary");
        return KeyValuePair.Create(enumMemberName, summaryNode?.Value.Trim());
    }
}