using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Точка маршрута
/// </summary>
public sealed class PointDto
{
    /// <summary>
    /// Идентификатор локациии
    /// </summary>
    public Guid LocationId { get; set; }

    /// <summary>
    /// Тип точки
    /// </summary>
    public PointType Type { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    public int Order { get; set; }
}