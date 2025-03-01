using System.IO.Abstractions.TestingHelpers;
using BXCP.ProgrammingChallenge.Adapters.Csv;
using BXCP.ProgrammingChallenge.Interfaces;
using Castle.Core.Logging;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BXCP.ProgrammingChallenge.Tests.Countries;

public class CsvCountryReaderTests
{
    private ILogger<ICountryReader> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new NullLogger<ICountryReader>();
    }

    [Test]
    public void ReadCountries_FileDoesNotExist()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var fileName = "doesnotexist.csv";
        var sut = new CsvCountryReader(fileSystem, _logger);

        // Act
        var result = sut.ReadCountries(fileName);

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError($"file not found at specified path: {fileName}");
    }

    [Test]
    public void ReadCountries_EmptyFile()
    {
        // Arrange
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "empty.csv", new MockFileData(string.Empty)}
        });
        var sut = new CsvCountryReader(fileSystem, _logger);

        // Act
        var result = sut.ReadCountries("empty.csv");

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeEmpty();
    }

    [Test]
    public void ReadCountries_IncorrectFileExtension()
    {
        // Arrange
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "empty.txt", new MockFileData(string.Empty)}
        });
        var sut = new CsvCountryReader(fileSystem, _logger);

        // Act
        var result = sut.ReadCountries("empty.txt");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError("file uses incompatible extension for csv");
    }

    [Test]
    public void ReadWeatherRecords_GermanNumberFormat()
    {
        // Arrange
        var testFile = TestHelper.ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.countries.csv");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "countries.csv", new MockFileData(testFile)}
        });
        var sut = new CsvCountryReader(fileSystem, _logger);

        // Act
        var result = sut.ReadCountries("countries.csv");

        // Assert
        result.Should().BeSuccess();
        result.Value.Count().Should().Be(27);
    }

    [Test]
    public void ReadCountries_MissingColumn()
    {
        // Arrange
        var testFile = TestHelper.ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.countries-missing-column.csv");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "countries-missing-columns.csv", new MockFileData(testFile)}
        });
        var sut = new CsvCountryReader(fileSystem, _logger);

        // Act
        var result = sut.ReadCountries("countries-missing-columns.csv");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError("could not parse csv file into weather record object");
    }

    [Test]
    public void ReadCountries_MissingValue()
    {
        // Arrange
        var testFile = TestHelper.ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.countries-missing-value.csv");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "countries-missing-columns.csv", new MockFileData(testFile)}
        });
        var sut = new CsvCountryReader(fileSystem, _logger);

        // Act
        var result = sut.ReadCountries("countries-missing-columns.csv");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError("could not parse csv file into weather record object");
    }
}
