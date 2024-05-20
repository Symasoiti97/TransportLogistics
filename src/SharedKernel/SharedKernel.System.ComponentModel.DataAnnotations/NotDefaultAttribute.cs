using System.Text.Json;

namespace System.ComponentModel.DataAnnotations;

public sealed class NotDefaultAttribute : ValidationAttribute
{
    public override string FormatErrorMessage(string name)
    {
        var propertyName = JsonNamingPolicy.CamelCase.ConvertName(name);
        return $"The property `{propertyName}` cannot be empty.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return false;
        }

        var type = value.GetType();

        return !type.IsValueType || !Activator.CreateInstance(type)!.Equals(value);
    }
}