using FluentAssertions;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using Xunit;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

public sealed class ComparatorTests
{
    [Theory]
    [MemberData(
        nameof(ComparatorTestsData.CompareTariffs_Test_Data),
        MemberType = typeof(ComparatorTestsData))]
    public void CompareTariffs_Test(Tariff srcTariff, Tariff destTariff, bool isEquals)
    {
        if (isEquals)
        {
            srcTariff.Should().BeEquivalentTo(destTariff);
        }
        else
        {
            srcTariff.Should().NotBeEquivalentTo(destTariff);
        }

        srcTariff.Equals(destTariff).Should().Be(isEquals);
    }

    [Theory]
    [MemberData(
        nameof(ComparatorTestsData.CompareRoutes_Test_Data),
        MemberType = typeof(ComparatorTestsData))]
    public void CompareRoutes_Test(Route srcRoute, Route destRoute, bool isEquals)
        => srcRoute.Equals(destRoute).Should().Be(isEquals);
}
