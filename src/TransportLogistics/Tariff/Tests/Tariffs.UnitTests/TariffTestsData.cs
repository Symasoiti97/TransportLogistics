using System;
using System.Collections.Generic;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class TariffTestsData
{
    private static readonly Location LocationWorld = Location.World(Guid.NewGuid());

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Negative_Test_Data()
    {
        var route = new Route(
            new[]
            {
                Point.Fob(Location.Country(Guid.NewGuid(), LocationWorld), 1),
                Point.Fob(Location.Country(Guid.NewGuid(), LocationWorld), 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            route);

        yield return new[]
        {
            tariff1
        };
    }

    public static IEnumerable<Tariff[]> TariffCopyAsReal_Positive_Test_Data()
    {
        var route = new Route(
            new[]
            {
                Point.Fob(Location.Country(Guid.NewGuid(), LocationWorld), 1),
                Point.Fob(Location.Country(Guid.NewGuid(), LocationWorld), 2)
            });

        var tariff1 = Tariff.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            route,
            ContainerOwn.Coc,
            ContainerSize.S20,
            CargoType.Heavy,
            new Price(129, "USD"));

        yield return new[]
        {
            tariff1
        };
    }
}