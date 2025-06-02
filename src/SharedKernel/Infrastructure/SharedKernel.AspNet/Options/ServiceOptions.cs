namespace TL.SharedKernel.Infrastructure.AspNet.Options;

public sealed class ApiOptions
{
    public ApiOptions(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }

    public string Name { get; }
}