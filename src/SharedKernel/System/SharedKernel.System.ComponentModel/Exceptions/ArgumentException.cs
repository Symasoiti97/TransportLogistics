using System.Runtime.CompilerServices;

namespace System.ComponentModel.Exceptions;

public static class ArgumentException
{
    /// <summary>
    /// Throws if the value equals its default value
    /// </summary>
    /// <param name="value">Value</param>
    /// <param name="paramName">Value name</param>
    /// <typeparam name="T">Value type</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfDefault<T>(
        T value,
        [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (Equals(value, default(T)))
        {
            throw new System.ArgumentException(
                $"Value cannot be default. (Parameter '{paramName}')",
                paramName);
        }
    }
}
