using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using TL.SharedKernel.Business.Aggregates;
using AssemblyReference = TL.Socials.Identity.Business.Aggregates.AssemblyReference;

namespace TL.Socials.Identity.Startups.Api.BackgroundServices;

public sealed class EventTypeInfoResolver : DefaultJsonTypeInfoResolver
{
    private static readonly Dictionary<string, Type> KnownTypes;

    static EventTypeInfoResolver()
    {
        KnownTypes = typeof(AssemblyReference).Assembly
            .GetTypes()
            .Where(type => typeof(Event).IsAssignableFrom(type) && !type.IsAbstract)
            .ToDictionary(type => type.Name, type => type);
    }

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);

        if (type == typeof(Event))
        {
            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions { TypeDiscriminatorPropertyName = "type" };

            foreach (var jsonDerivedType in KnownTypes
                         .Select(kvp => new JsonDerivedType(kvp.Value, kvp.Key)))
            {
                typeInfo.PolymorphismOptions.DerivedTypes.Add(jsonDerivedType);
            }
        }

        return typeInfo;
    }
}
