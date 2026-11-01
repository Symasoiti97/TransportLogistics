using System.Runtime.CompilerServices;

namespace System.ComponentModel.Exceptions;

public static class InvalidEnumArgumentException
{
    /// <summary>
    /// Throws if the enum value is not defined
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="paramName">Value name</param>
    /// <typeparam name="T">Value type</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfUndefined<T>(
        T value,
        [CallerArgumentExpression("value")] string? paramName = null) where T : struct, Enum
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(paramName);

        if (!Enum.IsDefined(value))
        {
            throw new ComponentModel.InvalidEnumArgumentException(
                $"Value can not be undefined. (Parameter '{paramName}')");
        }
    }
}
