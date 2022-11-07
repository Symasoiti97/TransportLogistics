using Application.Abstracts;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Команда для создания тарифа
/// </summary>
public sealed class CreateTariffCommand : ICommand
{
    /// <summary>
    /// Идентификатор сущности
    /// </summary>
    public Guid TariffId { get; set; }

    /// <summary>
    /// Идентификатор профиля. Менеджер профиля
    /// </summary>
    public Guid ManagerProfileId { get; set; }
}