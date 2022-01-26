﻿using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using TL.Tariff.Tariff;
using Xunit;

namespace TL.UnitTests;

public class ComparatorTests
{
    [Fact]
    public void CompareTwoObjectWithoutEqualsTest_ShouldBeEqual()
    {
        var tariff1 = new Tariff.Tariff.Tariff()
        {
            Id = new Guid("51D6A730-7B45-4DC1-9C81-5CB8D9E8D46E"),
            Own = ContainerOwn.Soc
        };
        
        var tariff2 = new Tariff.Tariff.Tariff()
        {
            Id = new Guid("51D6A730-7B45-4DC1-9C81-5CB8D9E8D46E"),
            Own = ContainerOwn.Soc
        };

        tariff1.Should().BeEquivalentTo(tariff2);
    }
    
    [Theory, AutoData]
    public void CompareTwoObjectWithoutEqualsTest_ShouldNotBeEqual(Tariff.Tariff.Tariff tariff1, Tariff.Tariff.Tariff tariff2)
    {
        tariff1.Should().NotBeEquivalentTo(tariff2);
    }
}