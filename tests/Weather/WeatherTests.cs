using BXCP.ProgrammingChallenge.Core.Models;
using FluentAssertions;

namespace BXCP.ProgrammingChallenge.Tests.Weather;

public class WeatherRecordTests
{
    [Test]
    public void TemperatureSpread_SameValues()
    {
        // Arrange
        var sut = new WeatherRecord()
        {
            Day = 1,
            MaximumTemperature = 100,
            MinimumTemperature = 100,
        };

        // Assert
        sut.TemperatureSpread.Should().Be(0);
    }

    [Test]
    public void TemperatureSpread_MinGreaterThanMax()
    {
        // Arrange
        var sut = new WeatherRecord()
        {
            Day = 1,
            MaximumTemperature = 50,
            MinimumTemperature = 100,
        };

        // Assert
        sut.TemperatureSpread.Should().Be(50);
    }

    [Test]
    public void ToString_Successful()
    {
        // Arrange
        var sut = new WeatherRecord()
        {
            Day = 1,
            MaximumTemperature = 50,
            MinimumTemperature = 100,
        };

        // Assert
        sut.ToString().Should().Be("Day: 1, Maximum Temperature: 50, Minimum Temperature: 100, Temperature Spread: 50");
    }
}
