using AutoFixture;
using BXCP.ProgrammingChallenge.Core.Models;
using FluentAssertions;

namespace BXCP.ProgrammingChallenge.Tests.Countries;

public class CountryTests
{
    private IFixture _fixture;

    public void Setup()
    {
        _fixture = new Fixture();
    }

    [Test]
    public void PopulationDensity_SameValues()
    {
        // Arrange
        var sut = new Country()
        {
            Name = "test",
            Population = 100,
            Area = 100,
        };

        // Assert
        sut.PopulationDensity.Should().Be(1);
    }

    [Test]
    public void PopulationDensity_DivisionByZero()
    {
        // Arrange
        var sut = new Country()
        {
            Name = "test",
            Population = 100,
            Area = 0,
        };

        // Assert
        sut.PopulationDensity.Should().Be(double.NaN);
    }

    [Test]
    public void PopulationDensity_CountrySmallerThanOneSquareKilometer()
    {
        // Arrange
        var sut = new Country()
        {
            Name = "test",
            Population = 100,
            Area = 0.1,
        };

        // Assert
        sut.PopulationDensity.Should().Be(1000);
    }

    [Test]
    public void ToString_Successful()
    {
        // Arrange
        var sut = new Country()
        {
            Name = "test",
            Population = 1000,
            Area = 1,
        };

        // Assert
        sut.ToString().Should().Be("Country: test, Population: 1000, Area: 1, Population Density: 1000");
    }
}
