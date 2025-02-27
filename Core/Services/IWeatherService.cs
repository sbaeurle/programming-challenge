using BXCP.ProgrammingChallenge.Core.Models;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Services;

public interface IWeatherService
{
  public Result<WeatherRecord> GetDayWithLeastTemperatureSpread(string source);
}
