using JetBrains.Annotations;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Infrastructure.WebApi.Controllers.Tariff.Dto;

/// <summary>
/// Точка маршрута
/// </summary>
[PublicAPI]
public class PointDto
{
    /// <summary>
    /// Идентификатор локации
    /// </summary>
    /// <example>a6883df6-e66f-41c3-b001-722629b38a04</example>
    public Guid LocationId { get; set; }

    /// <summary>
    /// Тип точки
    /// </summary>
    /// <example>Fob</example>
    public PointType Type { get; set; }

    /// <summary>
    /// Порядковый номер точки
    /// </summary>
    /// <example>1</example>
    public int Order { get; set; }
}