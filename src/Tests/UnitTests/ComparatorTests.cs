using System;
using AutoFixture.Xunit2;
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

        var worldLocation = new Location(Guid.NewGuid(), null, LocationType.World, new LocalizedText("World", "Мир", "World"));
        var russiaLocation = new Location(Guid.NewGuid(), worldLocation, LocationType.Country, new LocalizedText("Russia", "Россия", "Россия"));
        var chinaLocation = new Location(Guid.NewGuid(), worldLocation, LocationType.Country, new LocalizedText("China", "Китай", "中國"));

        var points = new Point[]
        {
            new(russiaLocation, PointType.Fot, 1),
            new(chinaLocation, PointType.Fot, 2)
        };
        var route = new Route(points);

        var price = new Price(1200, "USD");

        var tariff1 = new Tariff(tariffId, managerProfileId, route, ContainerOwn.Soc, ContainerSize.S20, CargoType.Bulk, price);
        var tariff2 = new Tariff(tariffId, managerProfileId, route, ContainerOwn.Soc, ContainerSize.S20, CargoType.Bulk, price);

        tariff1.Should().BeEquivalentTo(tariff2);
    }

    //todo не работает
    [Theory, AutoData]
    public void CompareTwoObjectWithoutEqualsTest_ShouldNotBeEqual(Tariff tariff1, Tariff tariff2)
    {
        tariff1.Should().NotBeEquivalentTo(tariff2);
    }
}