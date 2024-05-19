namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;

internal sealed class ServiceSettings
{
    public ServiceSettings(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}