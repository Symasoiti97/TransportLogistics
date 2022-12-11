using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Модель просмотра тарифа
/// </summary>
public sealed class TariffView
{
    /// <summary>
    /// Идентификатор тарифа
    /// </summary>
    /// <example>d8aae288-7ae9-4536-bc69-00e74cc85865</example>
    [NotDefault]
    public Guid Id { get; set; }

    /// <summary>
    /// Маршрут
    /// </summary>
    public RouteView? Route { get; set; }

    /// <summary>
    /// Груз
    /// </summary>
    public CargoType? CargoType { get; set; }

    /// <summary>
    /// Собственность контейнера
    /// </summary>
    public ContainerOwn? ContainerOwn { get; set; }

    /// <summary>
    /// Размер контейнера
    /// </summary>
    public ContainerSize? ContainerSize { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    public PriceView? Price { get; set; }

    /// <summary>
    /// Идентификатор профиля, менеджер текущего тарифа
    /// </summary>
    /// <example>5f1b7781-fc42-4b02-8e4d-b105de4ca85f</example>
    [NotDefault]
    public Guid ManagerProfileId { get; set; }

    /// <summary>
    /// Указывает тариф является черновиком или действующим
    /// True - тариф является черновиком
    /// False - тариф является действующим
    /// </summary>
    /// <example>true</example>
    [Required]
    public bool IsDraft { get; set; }
}