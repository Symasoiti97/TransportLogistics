using FluentAssertions;
using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using Xunit;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

public sealed class TariffTests
{
    [Theory]
    [MemberData(nameof(TariffTestsData.TariffCopyAsReal_Positive_Test_Data), MemberType = typeof(TariffTestsData))]
    public void TariffCopyAsReal_Positive_Test(Tariff tariff)
    {
        var publishTariffAction = tariff.SetAsReal;

        publishTariffAction.Should().NotThrow();
    }

    [Theory]
    [MemberData(nameof(TariffTestsData.TariffCopyAsReal_Negative_Test_Data), MemberType = typeof(TariffTestsData))]
    public void TariffCopyAsReal_Negative_Test(Tariff tariff)
    {
        var publishTariffAction = tariff.SetAsReal;

        publishTariffAction.Should().Throw<Exception>();
    }

    [Fact]
    public void CreateTariff_Negative_Test()
    {
        var createTariffAction1 = () =>
        {
            var tariffRoute2 = new Route(
                new HashSet<Point>
                {
                    Point.Fob(Guid.NewGuid(), 1),
                    Point.Fob(Guid.NewGuid(), 3)
                });

            Tariff.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                tariffRoute2);
        };

        createTariffAction1.Should().Throw<ErrorException>().And.Error.Should().BeOfType<Conflict>();

        var createTariffAction2 = () =>
        {
            var tariffRoute2 = new Route(
                new HashSet<Point>
                {
                    Point.Fob(Guid.NewGuid(), 1),
                    Point.Fob(Guid.NewGuid(), 2),
                    Point.Fob(Guid.NewGuid(), 2)
                });

            Tariff.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                tariffRoute2);
        };

        createTariffAction2.Should().Throw<ErrorException>().And.Error.Should().BeOfType<Conflict>();
    }
}