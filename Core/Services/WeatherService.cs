using BXCP.ProgrammingChallenge.Core.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Services;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Core.Services;

internal class WeatherService(IDictionary<string, IWeatherReader> _readers) : IWeatherService
{
  public Result<WeatherRecord> GetDayWithLeastTemperatureSpread(string source)
  {
    var ext = Path.GetExtension(source);

    if (!_readers.TryGetValue(ext, out var reader))
    {
      return Result.Fail($"no reader specified for file type {ext}");
    }

    var weatherRecordsResult = reader.ReadWeatherRecords(source);

    if (weatherRecordsResult.IsFailed)
    {
      return Result.Fail(new Error($"could not extract weather records from source {source}").CausedBy(weatherRecordsResult.Errors));
    }

    var weatherRecords = weatherRecordsResult.Value;

    if (!weatherRecords.Any())
    {
      return Result.Fail("empty collection may yield no answer");
    }

    var result = weatherRecords.MinBy(x => x.TemperatureSpread);

    // if statement included for theoretical nullability check, however there is not realistic circumstance this could happen
    if (result is null)
    {
      return Result.Fail("calculation returned no value");
    }

    return Result.Ok(result);
  }
}
