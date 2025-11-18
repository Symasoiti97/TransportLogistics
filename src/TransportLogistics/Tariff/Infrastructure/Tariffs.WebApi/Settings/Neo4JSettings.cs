using TL.SharedKernel.Infrastructure.DependencyInjection.Settings;

namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;

internal sealed class Neo4JSettings : INeo4JSettings
{
    public Uri Uri { get; }
    public string UserName { get; }
    public string Password { get; }

    public Neo4JSettings(Uri uri, string userName, string password)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        Uri = uri;
        UserName = userName;
        Password = password;
    }
}