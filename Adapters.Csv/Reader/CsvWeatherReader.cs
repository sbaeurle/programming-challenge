using System.Globalization;
using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Adapters.Csv.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using CsvHelper;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BXCP.ProgrammingChallenge.Adapters.Csv;

public class CsvWeatherReader(IFileSystem _fileSystem, ILogger logger) : IWeatherReader
{
  public Result<IEnumerable<Core.Models.WeatherRecord>> ReadWeatherRecords(string source)
  {
    source = FileHelper.StripFileProtocol(source);

    logger.LogDebug("Calculated location for file: {Location}", source);

    if (!_fileSystem.File.Exists(source))
    {
      logger.LogError("File does not exist at {Source}", source);
      return Result.Fail(new Error($"file not found at specified path: {source}"));
    }

    if (_fileSystem.Path.GetExtension(source) != ".csv")
    {
      logger.LogError("File extension {Ext} does not fit reader", _fileSystem.Path.GetExtension(source));
      return Result.Fail("file uses incompatible extension for csv");
    }

    using var fileStream = _fileSystem.File.OpenRead(source);
    using var reader = new StreamReader(fileStream);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    try
    {
      var records = csv.GetRecords<WeatherRecord>().ToList(); // ToList is necessary to materialize the list

      return Result.Ok(records.Select(x => new Core.Models.WeatherRecord
      {
        Day = x.Day,
        MaximumTemperature = x.MaximumTemperature,
        MinimumTemperature = x.MinimumTemperature
      }));
    }
    catch (CsvHelperException ex)
    {
      logger.LogError(ex, "Reading weather records from {File} failed", source);
      return Result.Fail(new Error("could not parse csv file into weather record object").CausedBy(ex));
    }
  }
}
