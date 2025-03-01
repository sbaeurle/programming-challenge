using BXCP.ProgrammingChallenge.Core.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Services;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BXCP.ProgrammingChallenge.Core.Services;

internal class WeatherService(IDictionary<string, IWeatherReader> _readers, ILogger logger) : IWeatherService
{
  public Result<WeatherRecord> GetDayWithLeastTemperatureSpread(string source)
  {
    var ext = Path.GetExtension(source);

    logger.LogDebug("Calculate day with lest temperature spread for {Source} with {Extension}", source, ext);

    if (!_readers.TryGetValue(ext, out var reader))
    {
      logger.LogError("No reader specified for file with {Extension}", ext);
      return Result.Fail($"no reader specified for file type {ext}");
    }

    var weatherRecordsResult = reader.ReadWeatherRecords(source);

    if (weatherRecordsResult.IsFailed)
    {
      logger.LogError("Reading weather records failed");
      return Result.Fail(new Error($"could not extract weather records from source {source}").CausedBy(weatherRecordsResult.Errors));
    }

    logger.LogInformation("Successfully retrieved weather records from {Source}", source);

    var weatherRecords = weatherRecordsResult.Value;

    if (logger.IsEnabled(LogLevel.Debug))
    {
      logger.LogDebug("Retrieved {Count} weather records from data source", weatherRecords.Count());
    }

    if (!weatherRecords.Any())
    {
      logger.LogError("Collection of weather records is empty. Can't calculate highest population density");
      return Result.Fail("empty collection may yield no answer");
    }

    var result = weatherRecords.MinBy(x => x.TemperatureSpread);

    // if statement included for theoretical nullability check, however there is not realistic circumstance this could happen
    if (result is null)
    {
      logger.LogError("Calculation of least temperature spread returned no value");
      return Result.Fail("calculation returned no value");
    }

    logger.LogInformation("Successfully calculated day with least temperature spread");

    return Result.Ok(result);
  }
}
