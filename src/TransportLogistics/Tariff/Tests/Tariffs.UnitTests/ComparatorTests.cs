using FluentAssertions;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using Xunit;

namespace TL.TransportLogistics.Tariffs.Startups.Tests.UnitTests;

public sealed class ComparatorTests
{
    [Theory]
    [MemberData(
        nameof(ComparatorTestsData.CompareTwoObjectWithoutEquals_Test_Data),
        MemberType = typeof(ComparatorTestsData))]
    public void CompareTwoTariffsWithoutEquals_Test(Tariff srcTariff, Tariff destTariff, bool isEquals)
    {
        if (isEquals)
        {
            srcTariff.Should().BeEquivalentTo(destTariff);
        }
        else
        {
            srcTariff.Should().NotBeEquivalentTo(destTariff);
        }
    }
}