using System.Runtime.CompilerServices;

namespace System.ComponentModel.Exceptions;

public static class ArgumentException
{
    /// <summary>
    /// Выбрасывает исключение если значения является значением по умолчанию
    /// </summary>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
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
