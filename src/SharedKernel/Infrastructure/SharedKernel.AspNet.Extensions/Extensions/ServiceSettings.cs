namespace TL.SharedKernel.Infrastructure.AspNet.Extensions.Middlewares.Extensions;

public sealed class ApiSettings
{
    public ApiSettings(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}