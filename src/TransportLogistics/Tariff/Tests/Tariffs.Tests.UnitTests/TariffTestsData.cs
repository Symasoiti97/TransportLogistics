using System;
using System.Collections.Generic;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Tests.UnitTests;

internal static class TariffTestsData
{
    private static readonly Location LocationWorld = Location.World(Guid.NewGuid());

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Negative_Test_Data()
    {
        var tariff1 = new Tariff(id: Guid.NewGuid(), managerProfileId: Guid.NewGuid());
        tariff1.SetRoute(new Route(new[]
        {
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 1),
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 2)
        }));

        yield return new[]
        {
            tariff1
        };
    }

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Positive_Test_Data()
    {
        var tariff1 = new Tariff(id: Guid.NewGuid(), managerProfileId: Guid.NewGuid());
        tariff1.SetRoute(new Route(new[]
        {
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 1),
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 2)
        }));
        tariff1.SetCargoType(CargoType.Heavy);
        tariff1.SetContainerOwn(ContainerOwn.Coc);
        tariff1.SetContainerSize(ContainerSize.S20);
        tariff1.SetPrice(new Price(129, "USD"));

        yield return new[]
        {
            tariff1
        };
    }
}