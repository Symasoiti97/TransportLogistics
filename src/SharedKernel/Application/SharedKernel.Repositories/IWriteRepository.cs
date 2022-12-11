namespace TL.SharedKernel.Application.Repositories;

/// <summary>
/// Репозиторий для сохранения изменений
/// </summary>
public interface IWriteRepository
{
    /// <summary>
    /// Сохранить изменения репозитория
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}