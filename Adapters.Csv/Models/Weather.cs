using CsvHelper.Configuration.Attributes;

namespace BXCP.ProgrammingChallenge.Adapters.Csv.Models;

public class WeatherRecord
{
  [Name("Day")]
  public required uint Day { get; set; }

  [Name("MxT")]
  public required int MaximumTemperature { get; set; }

  [Name("MnT")]
  public required int MinimumTemperature { get; set; }
}


