using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Application.Commands;

/// <summary>
/// Use-case handler, used for both commands and queries
/// </summary>
/// <typeparam name="TUseCase">Query</typeparam>
/// <typeparam name="TResult">Query result</typeparam>
public interface IUseCaseHandler<in TUseCase, TResult> where TUseCase : IUseCase<TResult>
{
    Task<TResult> HandleAsync(TUseCase command, CancellationToken cancellationToken);
}

/// <summary>
/// Use-case handler, used for both commands and events
/// </summary>
/// <typeparam name="TUseCase">Command type</typeparam>
public interface IUseCaseHandler<in TUseCase> : IUseCaseHandler where TUseCase : IUseCase
{
    Task HandleAsync(TUseCase command, CancellationToken cancellationToken);

    Task IUseCaseHandler.HandleAsync(object command, CancellationToken cancellationToken) =>
        HandleAsync((TUseCase)command, cancellationToken);
}

/// <summary>
/// Use-case handler, used for both commands and events
/// </summary>
public interface IUseCaseHandler
{
    Task HandleAsync(object command, CancellationToken cancellationToken);
}
