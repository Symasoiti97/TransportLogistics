using System;
using Domain.Tariff.AggregateTariff;
using FluentAssertions;
using Xunit;

namespace UnitTests;

public class ComparatorTests
{
    [Fact]
    public void CompareTwoObjectWithoutEqualsTest_ShouldBeEqual()
    {
        var tariffId = Guid.NewGuid();
        var managerProfileId = Guid.NewGuid();

        var worldLocation = Location.World(Guid.NewGuid());
        var firstCountryLocation = Location.Country(Guid.NewGuid(), worldLocation);
        var secondCountryLocation = Location.Country(Guid.NewGuid(), worldLocation);

        var points = new[]
        {
            Point.Fot(firstCountryLocation, 1),
            Point.Fot(secondCountryLocation, 2)
        };
        var route = new Route(points);
        var price = new Price(1200, "USD");

        var tariff1 = new Tariff(tariffId, managerProfileId, route, ContainerOwn.Soc, ContainerSize.S20, CargoType.Bulk, price);
        var tariff2 = new Tariff(tariffId, managerProfileId, route, ContainerOwn.Soc, ContainerSize.S20, CargoType.Bulk, price);

        tariff1.Should().BeEquivalentTo(tariff2);
    }

    [Fact]
    public void CompareTwoObjectWithoutEqualsTest_ShouldNotBeEqual()
    {
        var managerProfileId = Guid.NewGuid();

        var worldLocation = Location.World(Guid.NewGuid());
        var firstCountryLocation = Location.Country(Guid.NewGuid(), worldLocation);
        var secondCountryLocation = Location.Country(Guid.NewGuid(), worldLocation);

        var points = new[]
        {
            Point.Fot(firstCountryLocation, 1),
            Point.Fot(secondCountryLocation, 2)
        };
        var route = new Route(points);
        var price = new Price(1200, "USD");

        var tariff1 = new Tariff(Guid.NewGuid(), managerProfileId, route, ContainerOwn.Soc, ContainerSize.S20, CargoType.Bulk, price);
        var tariff2 = new Tariff(Guid.NewGuid(), managerProfileId, route, ContainerOwn.Soc, ContainerSize.S20, CargoType.Bulk, price);

        tariff1.Should().NotBeEquivalentTo(tariff2);
    }
}