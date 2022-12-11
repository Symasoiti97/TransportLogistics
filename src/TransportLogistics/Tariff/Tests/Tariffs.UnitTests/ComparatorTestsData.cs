using System;
using System.Collections.Generic;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

internal static class ComparatorTestsData
{
    private static readonly Location LocationWorld = Location.World(Guid.NewGuid());

    public static IEnumerable<object> CompareTwoObjectWithoutEquals_Test_Data()
    {
        var tariffId1 = Guid.NewGuid();
        var managerProfileId1 = Guid.NewGuid();

        var firstCountryLocation1 = Location.Country(Guid.NewGuid(), LocationWorld);
        var secondCountryLocation1 = Location.Country(Guid.NewGuid(), LocationWorld);

        var points1 = new[]
        {
            Point.Fot(firstCountryLocation1, 1),
            Point.Fot(secondCountryLocation1, 2)
        };
        var route1 = new Route(points1);
        var price1 = new Price(1200, "USD");

        var srcTariff1 = new Tariff(
            tariffId1,
            managerProfileId1,
            route1,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price1);
        var destTariff1 = new Tariff(
            tariffId1,
            managerProfileId1,
            route1,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price1);

        yield return new object[]
        {
            srcTariff1, destTariff1, true
        };

        var managerProfileId2 = Guid.NewGuid();

        var firstCountryLocation2 = Location.Country(Guid.NewGuid(), LocationWorld);
        var secondCountryLocation2 = Location.Country(Guid.NewGuid(), LocationWorld);

        var points2 = new[]
        {
            Point.Fot(firstCountryLocation2, 1),
            Point.Fot(secondCountryLocation2, 2)
        };
        var route2 = new Route(points2);
        var price2 = new Price(1200, "USD");

        var srcTariff2 = new Tariff(
            Guid.NewGuid(),
            managerProfileId2,
            route2,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price2);
        var destTariff2 = new Tariff(
            Guid.NewGuid(),
            managerProfileId2,
            route2,
            ContainerOwn.Soc,
            ContainerSize.S20,
            CargoType.Bulk,
            price2);

        yield return new object[]
        {
            srcTariff2, destTariff2, false
        };
    }
}