using System;
using BXCP.ProgrammingChallenge.Models;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Interfaces;

public interface IWeatherService
{
  public Result<WeatherRecord> GetDayWithLeastTemperatureSpread(IEnumerable<WeatherRecord> weatherRecords);
}
