namespace TL.SharedKernel.Business.Aggregates;

/// <summary>
/// Сценарий, бывает двух типов: команда и запрос
/// </summary>
/// <typeparam name="TResult">Результат запроса</typeparam>
// ReSharper disable once UnusedTypeParameter
public interface IUseCase<out TResult>;

/// <summary>
/// Сценарий, который используется для команд
/// </summary>
public interface IUseCase;
