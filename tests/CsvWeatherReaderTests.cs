using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using BXCP.ProgrammingChallenge.Adapters.Csv;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;

namespace BXCP.ProgrammingChallenge.Tests;

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
        var testFile = ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.missing-column.csv");
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
        var testFile = ReadEmbeddedResource("BXCP.ProgrammingChallenge.Tests.Resources.missing-value.csv");
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

    private static string ReadEmbeddedResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
