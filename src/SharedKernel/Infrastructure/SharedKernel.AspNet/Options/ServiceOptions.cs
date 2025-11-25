namespace TL.SharedKernel.Infrastructure.AspNet.Options;

public sealed class ApiOptions
{
    public string Name { get; }

    public ApiOptions(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
    }
}
