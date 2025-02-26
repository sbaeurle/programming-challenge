using System;
using CsvHelper.Configuration.Attributes;

namespace BXCP.ProgrammingChallenge.Models;

public class WeatherRecord
{
  [Name("Day")]
  public int Day { get; set; }
  [Name("MxT")]
  public int MaximumTemperature { get; set; }
  [Name("MnT")]
  public int MinimumTemperature { get; set; }
}
