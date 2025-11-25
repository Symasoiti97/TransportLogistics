using Microsoft.Extensions.Configuration;

namespace TL.SharedKernel.Infrastructure.AspNet.Extensions;

public static class ConfigurationExtensions
{
    public static T GetRequiredSectionValue<T>(this IConfiguration configuration, string sectionName) =>
        configuration.GetRequiredSection(sectionName).Get<T>()
        ?? throw new InvalidOperationException($"Section {sectionName} not found");

    public static string GetRequiredConnectionString(this IConfiguration configuration, string key) =>
        configuration.GetConnectionString(key)
        ?? throw new InvalidOperationException($"ConnectionString {key} not found");
}
