using System.Runtime.CompilerServices;

namespace System.ComponentModel.Exceptions;

public static class InvalidEnumArgumentException
{
    /// <summary>
    /// Выбрасывает исключение если значение enum не определенное
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfUndefined<T>(
        T value,
        [CallerArgumentExpression("value")] string? paramName = null) where T : struct, Enum
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(paramName);

        if (!Enum.IsDefined(value))
        {
            throw new System.ComponentModel.InvalidEnumArgumentException(
                $"Value can not be undefined. (Parameter '{paramName}')");
        }
    }
}