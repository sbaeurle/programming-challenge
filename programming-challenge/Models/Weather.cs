using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;

namespace BXCP.ProgrammingChallenge.Models;

public class WeatherRecord
{
  [Name("Day")]
  public uint Day { get; set; }

  [Name("MxT")]
  [Range(-100, 100, ErrorMessage = "temperature expected in human liveable conditions")]
  public int MaximumTemperature { get; set; }

  [Name("MnT")]
  [Range(-100, 100, ErrorMessage = "temperature expected in human liveable conditions")]
  public int MinimumTemperature { get; set; }

  public int TemperatureSpread
  {
    get
    {
      // Calculate absolute to ensure delta in edge cases
      return Math.Abs(MaximumTemperature - MinimumTemperature);
    }
  }

  public override string ToString()
  {
    return $"Day: {Day}, Maximum Temperature {MaximumTemperature}, MinimumTemperature: {MinimumTemperature}, Temperature Spread: {TemperatureSpread}";
  }
}


