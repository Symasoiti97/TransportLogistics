namespace TL.SharedKernel.Application.Commands;

/// <summary>
/// Обработчик сценария, используется как для команд так и для запросов
/// </summary>
/// <typeparam name="TUseCase">Запрос</typeparam>
/// <typeparam name="TResult">Результат запроса</typeparam>
public interface IUseCaseHandler<in TUseCase, TResult> where TUseCase : IUseCase<TResult>
{
    Task<TResult> HandleAsync(TUseCase command, CancellationToken cancellationToken);
}

/// <summary>
/// Обработчик сценария, используется как для команд так и для запросов
/// </summary>
/// <typeparam name="TUseCase">Тип команды</typeparam>
public interface IUseCaseHandler<in TUseCase> where TUseCase : IUseCase
{
    Task HandleAsync(TUseCase command, CancellationToken cancellationToken);
}