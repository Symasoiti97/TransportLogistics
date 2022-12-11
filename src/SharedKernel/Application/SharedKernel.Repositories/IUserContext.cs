namespace TL.SharedKernel.Application.Repositories;

/// <summary>
/// Пользовательский контекст.
/// Использовать для получения данных авторизованного пользователя
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Получить идентификатор профиля пользователя
    /// </summary>
    /// <returns>Идентификатор профиля пользователя</returns>
    Guid GetProfileId();
}