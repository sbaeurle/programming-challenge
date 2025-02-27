using System.IO.Abstractions.TestingHelpers;
using BXCP.ProgrammingChallenge.Adapters.Csv;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;

namespace BXCP.ProgrammingChallenge.Tests.Weather;

public class CsvWeatherReaderTests
{
    [Test]
    public void ReadWeatherRecords_FileDoesNotExist()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        var fileName = "doesnotexist.csv";
        var sut = new CsvWeatherReader(fileSystem);

        // Act
        var result = sut.ReadWeatherRecords(fileName);

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError($"file not found at specified path: {fileName}");
    }

    [Test]
    public void ReadWeatherRecords_EmptyFile()
    {
        // Arrange
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "empty.csv", new MockFileData(string.Empty)}
        });
        var sut = new CsvWeatherReader(fileSystem);

        // Act
        var result = sut.ReadWeatherRecords("empty.csv");

        // Assert
        result.Should().BeSuccess();
        result.Value.Should().BeEmpty();
    }

    [Test]
    public void ReadWeatherRecords_IncorrectFileExtension()
    {
        // Arrange
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "empty.json", new MockFileData(string.Empty)}
        });
        var sut = new CsvWeatherReader(fileSystem);

        // Act
        var result = sut.ReadWeatherRecords("empty.json");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError("file uses incompatible extension for csv");
    }

    [Test]
    public void ReadWeatherRecords_MissingColumn()
    {
        // Arrange
        var testFile = TestHelper.ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.missing-column.csv");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "missing-columns.csv", new MockFileData(testFile)}
        });
        var sut = new CsvWeatherReader(fileSystem);

        // Act
        var result = sut.ReadWeatherRecords("missing-columns.csv");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError("could not parse csv file into weather record object");
    }

    [Test]
    public void ReadWeatherRecords_MissingValue()
    {
        // Arrange
        var testFile = TestHelper.ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.missing-value.csv");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "missing-columns.csv", new MockFileData(testFile)}
        });
        var sut = new CsvWeatherReader(fileSystem);

        // Act
        var result = sut.ReadWeatherRecords("missing-columns.csv");

        // Assert
        result.Should().BeFailure();
        result.Should().HaveError("could not parse csv file into weather record object");
    }

    [Test]
    public void ReadWeatherRecords_Successful()
    {
        // Arrange
        var testFile = TestHelper.ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.weather.csv");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            { "weather.csv", new MockFileData(testFile)}
        });
        var sut = new CsvWeatherReader(fileSystem);

        // Act
        var result = sut.ReadWeatherRecords("weather.csv");

        // Assert
        result.Should().BeSuccess();
        result.Value.Count().Should().Be(30);
    }
}
