using TL.SharedKernel.Business.Aggregates;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Оборудование груза
/// </summary>
public sealed class CargoEquipment
{
    /// <summary>
    /// Создает
    /// </summary>
    /// <param name="cargoType">Тип груза</param>
    /// <param name="containerOwn">Принадлежность груза</param>
    /// <param name="containerSize">Размер контейнера</param>
    public CargoEquipment(CargoType cargoType, ContainerOwn containerOwn, ContainerSize containerSize)
    {
        Error.Throw().IfUndefined(containerSize);
        Error.Throw().IfUndefined(containerOwn);
        Error.Throw().IfUndefined(containerSize);

        CargoType = cargoType;
        ContainerOwn = containerOwn;
        ContainerSize = containerSize;
    }

    /// <summary>
    /// Груз
    /// </summary>
    public CargoType? CargoType { get; }

    /// <summary>
    /// Собственность контейнера
    /// </summary>
    public ContainerOwn? ContainerOwn { get; }

    /// <summary>
    /// Размер контейнера
    /// </summary>
    public ContainerSize? ContainerSize { get; }
}