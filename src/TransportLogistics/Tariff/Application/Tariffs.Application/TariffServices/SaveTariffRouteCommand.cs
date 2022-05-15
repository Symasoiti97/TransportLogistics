using Application.Abstracts;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Сохранить тариф с параметрами маршрута
/// </summary>
public class SaveTariffRouteCommand : ICommand
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    public Guid TariffId { get; set; }

    /// <summary>
    /// Точки маршрута
    /// </summary>
    public PointDto[] Points { get; set; } = null!;

    /// <summary>
    /// Менеджер тарифа. Идентификатор профиля пользователя
    /// </summary>
    public Guid ManagerProfileId { get; set; }
}