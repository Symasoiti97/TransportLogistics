using System;
using FluentAssertions;
using Tariffs.Domain.AggregateTariff;
using Xunit;

namespace Tariffs.Tests.UnitTests;

public class TariffTests
{
    [Theory]
    [MemberData(nameof(TariffTestsData.TariffCopyAsReal_Positive_Test_Data), MemberType = typeof(TariffTestsData))]
    public void TariffCopyAsReal_Positive_Test(Tariff tariff)
    {
        var publishTariffAction = tariff.CopyAsReal;

        publishTariffAction.Should().NotThrow();
    }

    [Theory]
    [MemberData(nameof(TariffTestsData.TariffCopyAsReal_Negative_Test_Data), MemberType = typeof(TariffTestsData))]
    public void TariffCopyAsReal_Negative_Test(Tariff tariff)
    {
        var publishTariffAction = tariff.CopyAsReal;

        publishTariffAction.Should().Throw<Exception>();
    }
}