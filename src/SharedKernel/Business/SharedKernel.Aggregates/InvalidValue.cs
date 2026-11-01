using System.ComponentModel.DataAnnotations;

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Invalid parameter error
/// </summary>
public sealed class InvalidValue : Error
{
    public override string Message => "Invalid value.";

    /// <summary>
    /// Value name
    /// </summary>
    /// <example>Route</example>
    [Required]
    public string Name { get; }

    /// <summary>
    /// Invalid value
    /// </summary>
    /// <example>null</example>
    public object? Value { get; }

    public InvalidValue(object? value, string name)
    {
        Value = value;
        Name = name;
    }
}
