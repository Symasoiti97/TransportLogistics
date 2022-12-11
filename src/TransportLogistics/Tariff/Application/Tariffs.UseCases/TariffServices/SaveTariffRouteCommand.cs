using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Сохранить тариф с параметрами маршрута
/// </summary>
/// <param name="TariffId">Идентификатор тарифа</param>
/// <param name="Points">Точки маршрута</param>
public sealed record SaveTariffRouteCommand(Guid TariffId, IEnumerable<PointDto> Points) : ICommand;