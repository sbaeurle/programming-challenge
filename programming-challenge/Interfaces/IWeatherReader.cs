using System;
using BXCP.ProgrammingChallenge.Models;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Interfaces;

public interface IWeatherReader
{
  public Result<IEnumerable<WeatherRecord>> ReadWeatherRecords(string filepath);
}
