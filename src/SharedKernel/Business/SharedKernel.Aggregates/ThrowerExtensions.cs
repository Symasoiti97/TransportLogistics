using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using EnsureThat;

#pragma warning disable CS8777

namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Методы расширения для объявления ошибок
/// </summary>
public static class ThrowerExtensions
{
    /// <summary>
    /// Выбрасывает исключение, если значение равно null
    /// </summary>
    /// <param name="thrower">Thrower</param>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfNull<T>(
        this in Thrower thrower,
        [NotNull] T? value,
        [CallerArgumentExpression("value")] string? paramName = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        if (value is null)
        {
            Thrower.Throw(
                new InvalidValue<T>(
                    value,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    $"Value can not be null. (Parameter '{paramName}')"));
        }

        return ref thrower;
    }

    /// <summary>
    /// Проверяет на null, если не null регестирует ошибку
    /// </summary>
    /// <param name="thrower">Thrower</param>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfNotNull<T>(
        this in Thrower thrower,
        T? value,
        [CallerArgumentExpression("value")] string? paramName = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        if (value is not null)
        {
            Thrower.Throw(
                new InvalidValue<T>(
                    value,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    $"Value can be null. (Parameter '{paramName}')"));
        }

        return ref thrower;
    }

    /// <summary>
    /// Проверяет на default, если default регестирует ошибку
    /// </summary>
    /// <param name="thrower">Строитель ошибок</param>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfDefault<T>(
        this in Thrower thrower,
        T value,
        [CallerArgumentExpression("value")] string? paramName = null) where T : struct
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        if (default(T).Equals(value))
        {
            Thrower.Throw(
                new InvalidValue<T>(
                    value,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    $"Value can not be default. (Parameter '{paramName}')"));
        }

        return ref thrower;
    }

    /// <summary>
    /// Проверяет определен ли enum, если не определен регестирует ошибку
    /// </summary>
    /// <param name="thrower">Строитель ошибок</param>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfUndefined<T>(
        this in Thrower thrower,
        T value,
        [CallerArgumentExpression("value")] string? paramName = null) where T : struct, Enum
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        if (!Enum.IsDefined(value))
        {
            Thrower.Throw(
                new InvalidValue<T>(
                    value,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    $"Value can not be undefined. (Parameter '{paramName}')"));
        }

        return ref thrower;
    }

    /// <summary>
    /// Выполняет предиктат, если вернул false, то регестирует ошибку
    /// </summary>
    /// <param name="thrower">Строитель ошибок</param>
    /// <param name="value">Значение</param>
    /// <param name="predicate">Предикат</param>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="paramName">Наименование значения</param>
    /// <typeparam name="T">Тип значения</typeparam>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfNot<T>(
        this in Thrower thrower,
        T value,
        Func<T, bool> predicate,
        string message,
        [CallerArgumentExpression("value")] string? paramName = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        if (!predicate(value))
        {
            Thrower.Throw(
                new InvalidValue<T>(
                    value,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    message));
        }

        return ref thrower;
    }

    /// <summary>
    /// Если строка пустая или содержит пустоту, то регестирует ошибку
    /// </summary>
    /// <param name="thrower">Строитель ошибок</param>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfNullOrWhiteSpace(
        this in Thrower thrower,
        string value,
        [CallerArgumentExpression("value")] string? paramName = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        if (string.IsNullOrWhiteSpace(value))
        {
            Thrower.Throw(
                new InvalidValue<string>(
                    value,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    $"Value '{paramName}' can't be empty"));
        }

        return ref thrower;
    }

    /// <summary>
    /// Выбрасывает <see cref="ErrorException"/> c ошибкой <see cref="InvalidValue"/>, если коллекция не имеет элементов
    /// </summary>
    /// <param name="thrower">Строитель ошибок</param>
    /// <param name="value">Значение</param>
    /// <param name="paramName">Наименование значения</param>
    /// <returns><paramref name="thrower"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly Thrower IfEmpty<T>(
        this in Thrower thrower,
        IEnumerable<T>? value,
        [CallerArgumentExpression("value")] string? paramName = null)
    {
        EnsureArg.IsNotNullOrWhiteSpace(paramName);

        var array = value as T[] ?? value?.ToArray();
        if (array?.Any() != true)
        {
            Thrower.Throw(
                new InvalidValue<T[]>(
                    array,
                    JsonNamingPolicy.CamelCase.ConvertName(paramName),
                    $"Collection '{paramName}' can't be empty"));
        }

        return ref thrower;
    }
}