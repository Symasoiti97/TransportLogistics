using System.Text.Json.Serialization.Metadata;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;

public static class ErrorJsonTypeInfoModifier
{
    public static void Modify(JsonTypeInfo typeInfo, IEnumerable<Type> subErrorTypes)
    {
        var baseType = typeof(Error);
        if (typeInfo.Type.IsSubclassOf(baseType))
        {
            foreach (var propertyInfo in typeInfo.Properties)
            {
                if (propertyInfo.Name is "details" or "message")
                {
                    propertyInfo.ShouldSerialize = (_, _) => false;
                }
            }
        }
        else if (typeInfo.Type == baseType)
        {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "type"
            };

            foreach (var subErrorType in subErrorTypes)
            {
                var jsonDerivedType = new JsonDerivedType(
                    subErrorType,
                    ErrorExtensions.BuildType(subErrorType));
                typeInfo.PolymorphismOptions.DerivedTypes.Add(jsonDerivedType);
            }
        }
    }
}