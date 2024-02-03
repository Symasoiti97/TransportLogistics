namespace TL.TransportLogistics.Tariffs.Startups.WebApi.Settings;

internal sealed class ServiceSettings
{
    public ServiceSettings(string name)
    {
        Name = name;
    }

    public string Name { get; }
}