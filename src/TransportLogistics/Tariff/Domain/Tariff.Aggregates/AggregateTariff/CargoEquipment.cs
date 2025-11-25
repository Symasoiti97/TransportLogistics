using System.ComponentModel.Exceptions;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

/// <summary>
/// Оборудование груза
/// </summary>
public sealed class CargoEquipment
{
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

    /// <summary>
    /// Создает
    /// </summary>
    /// <param name="cargoType">Тип груза</param>
    /// <param name="containerOwn">Принадлежность груза</param>
    /// <param name="containerSize">Размер контейнера</param>
    public CargoEquipment(CargoType cargoType, ContainerOwn containerOwn, ContainerSize containerSize)
    {
        InvalidEnumArgumentException.ThrowIfUndefined(cargoType);
        InvalidEnumArgumentException.ThrowIfUndefined(containerOwn);
        InvalidEnumArgumentException.ThrowIfUndefined(containerSize);

        CargoType = cargoType;
        ContainerOwn = containerOwn;
        ContainerSize = containerSize;
    }
}
