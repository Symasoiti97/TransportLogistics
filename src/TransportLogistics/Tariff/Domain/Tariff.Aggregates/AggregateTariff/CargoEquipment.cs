using System.ComponentModel.Exceptions;

namespace TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

public sealed class CargoEquipment
{
    public CargoType? CargoType { get; }

    public ContainerOwn? ContainerOwn { get; }

    public ContainerSize? ContainerSize { get; }

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
