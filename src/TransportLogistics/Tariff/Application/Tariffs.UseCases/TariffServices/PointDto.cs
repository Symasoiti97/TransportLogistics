using System.ComponentModel.DataAnnotations;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Точка маршрута
/// </summary>
public sealed class PointDto
{
    /// <summary>
    /// Идентификатор локациии
    /// </summary>
    /// <example>a6883df6-e66f-41c3-b001-722629b38a04</example>
    [NotDefault]
    public Guid LocationId { get; set; }

    /// <summary>
    /// Тип точки
    /// </summary>
    /// <example>Fob</example>
    [NotDefault]
    public PointType Type { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    /// <example>1</example>
    [NotDefault]
    public ushort Order { get; set; }
}
