using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Models;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Core;

public class WeatherService : IWeatherService
{
  public Result<WeatherRecord> GetDayWithLeastTemperatureSpread(IEnumerable<WeatherRecord> weatherRecords)
  {
    if (weatherRecords.Count() is 0)
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
