using AutoFixture;
using BXCP.ProgrammingChallenge.Core.Models;
using BXCP.ProgrammingChallenge.Core.Services;
using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Services;
using Castle.Core.Logging;
using FluentAssertions;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace BXCP.ProgrammingChallenge.Tests.Weather;

public class WeatherServiceTests
{
    private WeatherService _sut;
    private Mock<IWeatherReader> _mockWeatherReader;
    private IFixture _fixture;
    private ILogger<IWeatherService> _logger;

    [SetUp]
    public void Setup()
    {
        _mockWeatherReader = new Mock<IWeatherReader>();
        var readers = new Dictionary<string, IWeatherReader>{
            { ".test", _mockWeatherReader.Object}
        };
        _logger = new NullLogger<IWeatherService>();
        _sut = new WeatherService(readers, _logger);
        _fixture = new Fixture();
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_EmptyEnumerable()
    {
        // Arrange
        _mockWeatherReader
            .Setup(x => x.ReadWeatherRecords("empty.test"))
            .Returns(Result.Ok(Enumerable.Empty<WeatherRecord>()));

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread("empty.test");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("empty collection may yield no answer");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_NoReaderSpecified()
    {
        // Arrange

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread("empty.fail");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("no reader specified for file type .fail");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_ReaderFails()
    {
        // Arrange
        _mockWeatherReader
            .Setup(x => x.ReadWeatherRecords("fail.test"))
            .Returns(Result.Fail("test fails"));

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread("fail.test");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("could not extract weather records from source fail.test");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_SingleRecord()
    {
        // Arrange
        var testData = _fixture.CreateMany<WeatherRecord>(1);
        _mockWeatherReader
            .Setup(x => x.ReadWeatherRecords("singlerecord.test"))
            .Returns(Result.Ok(testData));

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread("singlerecord.test");

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
        _mockWeatherReader
            .Setup(x => x.ReadWeatherRecords("multiplerecords.test"))
            .Returns(Result.Ok(testData));

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread("multiplerecords.test");

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
        _mockWeatherReader
            .Setup(x => x.ReadWeatherRecords("samevalues.test"))
            .Returns(Result.Ok<IEnumerable<WeatherRecord>>(testData));

        var expected = testData.MinBy(x => x.TemperatureSpread);

        // Act
        var result = _sut.GetDayWithLeastTemperatureSpread("samevalues.test");

        // Assert
        result.Should().BeSuccess();
        result.Should().HaveValue(expected!);
    }
}
