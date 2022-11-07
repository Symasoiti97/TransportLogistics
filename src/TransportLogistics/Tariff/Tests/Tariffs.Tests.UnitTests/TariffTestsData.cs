using System;
using System.Collections.Generic;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Tests.UnitTests;

internal static class TariffTestsData
{
    private static readonly Location LocationWorld = Location.World(Guid.NewGuid());

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Negative_Test_Data()
    {
        var route = new Route(new[]
        {
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 1),
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 2)
        });

        var tariff1 = new Tariff(
            id: Guid.NewGuid(),
            managerProfileId: Guid.NewGuid(),
            route: route);

        yield return new[]
        {
            tariff1
        };
    }

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Positive_Test_Data()
    {
        var route = new Route(new[]
        {
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 1),
            Point.Fob(Location.Country(id: Guid.NewGuid(), parentLocation: LocationWorld), order: 2)
        });

        var tariff1 = new Tariff(
            id: Guid.NewGuid(),
            managerProfileId: Guid.NewGuid(),
            route: route,
            containerOwn: ContainerOwn.Coc,
            containerSize: ContainerSize.S20,
            cargoType: CargoType.Heavy,
            price: new Price(129, "USD"));

        yield return new[]
        {
            tariff1
        };
    }
}