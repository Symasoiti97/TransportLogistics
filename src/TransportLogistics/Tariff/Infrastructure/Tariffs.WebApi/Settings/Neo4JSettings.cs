using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection.Settings;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;

internal sealed class Neo4JSettings : INeo4JSettings
{
    public Neo4JSettings(Uri uri, string userName, string password)
    {
        Uri = uri;
        UserName = userName;
        Password = password;
    }

    public Uri Uri { get; }
    public string UserName { get; }
    public string Password { get; }
}