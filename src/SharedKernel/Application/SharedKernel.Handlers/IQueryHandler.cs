namespace TL.SharedKernel.Application.Commands;

/// <summary>
/// Обработчик запроса
/// </summary>
/// <typeparam name="TQuery">Запрос</typeparam>
/// <typeparam name="TResult">Результат запроса</typeparam>
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery command, CancellationToken cancellationToken = default);
}