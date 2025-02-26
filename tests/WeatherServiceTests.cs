using AutoFixture;
using BXCP.ProgrammingChallenge.Core;
using BXCP.ProgrammingChallenge.Models;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;

namespace BXCP.ProgrammingChallenge.Tests;

public class WeatherServiceTests
{
    private WeatherService _sut;
    private IFixture _fixture;

    [SetUp]
    public void Setup()
    {
        _sut = new WeatherService();
        _fixture = new Fixture();
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_EmptyEnumerable()
    {
        // Arrange

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread([]);

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("empty collection may yield no answer");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_SingleRecord()
    {
        // Arrange
        var testData = _fixture.CreateMany<WeatherRecord>(1);

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread(testData);

        // Assert
        result.Should().BeSuccess();
        result.Should().HaveValue(testData.First());
    }

    [TestCase(10)]
    [TestCase(100)]
    [TestCase(1000)]
    public void GetDayWithLeastTemperatureSpread_RandomRecords(int count)
    {
        // Arrange
        var testData = _fixture.CreateMany<WeatherRecord>(count);
        var expected = testData.MinBy(x => x.TemperatureSpread);

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread(testData);

        // Assert
        result.Should().BeSuccess();
        result.Should().HaveValue(expected!);
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_SameValues()
    {
        // Arrange
        var testData = new WeatherRecord[]{
            new() { Day = 1, MaximumTemperature = 100, MinimumTemperature = 95},
            new() { Day = 2, MaximumTemperature = 100, MinimumTemperature = 95},
        };

        var expected = testData.MinBy(x => x.TemperatureSpread);

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread(testData);

        // Assert
        result.Should().BeSuccess();
        result.Should().HaveValue(expected!);
    }
}
