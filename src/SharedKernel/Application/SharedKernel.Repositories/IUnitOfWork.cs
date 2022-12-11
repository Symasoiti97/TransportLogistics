namespace TL.SharedKernel.Application.Repositories;

public interface IUnitOfWork
{
    /// <summary>
    /// Получить репозиторий
    /// </summary>
    /// <typeparam name="TRepository">Тип репозитрия</typeparam>
    /// <returns>Репозиторий</returns>
    TRepository GetRepository<TRepository>() where TRepository : IRepository;

    /// <summary>
    /// Сохранить изменения репозиториев
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}