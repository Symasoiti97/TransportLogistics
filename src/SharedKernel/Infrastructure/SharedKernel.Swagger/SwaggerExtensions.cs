namespace TL.SharedKernel.Infrastructure.Swagger;

public static class SwaggerExtensions
{
    /// <summary>
    /// Build swagger schema name for the type
    /// </summary>
    /// <param name="type">Schema type</param>
    /// <returns>Schema name</returns>
    public static string BuildSwaggerSchemaName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        var genericParams = type.GetGenericArguments().Select(genericType => genericType.Name);
        var length = type.Name.IndexOf('`');
        return $"{type.Name.AsSpan(start: 0, length)}<{string.Join(',', genericParams)}>";
    }
}