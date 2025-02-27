using AutoFixture;
using BXCP.ProgrammingChallenge.Core.Models;
using BXCP.ProgrammingChallenge.Core.Services;
using BXCP.ProgrammingChallenge.Interfaces;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Moq;

namespace BXCP.ProgrammingChallenge.Tests.Countries;

public class CountryServiceTests
{
    private CountryService _sut;
    private Mock<ICountryReader> _mockCountryReader;
    private IFixture _fixture;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _mockCountryReader = new Mock<ICountryReader>();
        var readers = new Dictionary<string, ICountryReader>{
            { ".test", _mockCountryReader.Object}
        };
        _sut = new CountryService(readers);
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_EmptyEnumerable()
    {
        // Arrange
        _mockCountryReader
            .Setup(x => x.ReadCountries("empty.test"))
            .Returns(Result.Ok(Enumerable.Empty<Country>()));

        // Act
        var result = _sut.GetCountryWithHighestPopulationDensity("empty.test");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("empty collection may yield no answer");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_NoReaderSpecified()
    {
        // Arrange

        // Act
        var result = _sut.GetCountryWithHighestPopulationDensity("empty.fail");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("no reader specified for file type .fail");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_ReaderFails()
    {
        // Arrange
        _mockCountryReader
            .Setup(x => x.ReadCountries("fail.test"))
            .Returns(Result.Fail("test fails"));

        // Act
        var result = _sut.GetCountryWithHighestPopulationDensity("fail.test");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveReason("could not extract countries from source fail.test");
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_SingleRecord()
    {
        // Arrange
        var testData = _fixture.CreateMany<Country>(1);
        _mockCountryReader
            .Setup(x => x.ReadCountries("singlerecord.test"))
            .Returns(Result.Ok(testData));

        // Act
        var result = _sut.GetCountryWithHighestPopulationDensity("singlerecord.test");

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
        var testData = _fixture.CreateMany<Country>(count);
        var expected = testData.MaxBy(x => x.PopulationDensity);
        _mockCountryReader
            .Setup(x => x.ReadCountries("multiplerecords.test"))
            .Returns(Result.Ok(testData));

        // Act
        var result = _sut.GetCountryWithHighestPopulationDensity("multiplerecords.test");

        // Assert
        result.Should().BeSuccess();
        result.Should().HaveValue(expected!);
    }

    [Test]
    public void GetDayWithLeastTemperatureSpread_SameValues()
    {
        // Arrange
        var testData = new Country[]{
            new() { Name = "test1", Population = 1000, Area = 1},
            new() { Name = "test2", Population = 1000, Area = 1},
        };
        _mockCountryReader
            .Setup(x => x.ReadCountries("samevalues.test"))
            .Returns(Result.Ok<IEnumerable<Country>>(testData));

        // Act
        var result = _sut.GetCountryWithHighestPopulationDensity("samevalues.test");

        // Assert
        result.Should().BeSuccess();
        result.Should().HaveValue(testData[0]);
    }
}
