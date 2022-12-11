namespace System.ComponentModel.DataAnnotations;

public class NotDefaultAttribute : ValidationAttribute
{
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