using System.ComponentModel.DataAnnotations;

namespace BXCP.ProgrammingChallenge.Core.Models;

public class WeatherRecord
{
  public required uint Day { get; set; }

  [Range(-100, 100, ErrorMessage = "temperature expected in human liveable conditions")]
  public required int MaximumTemperature { get; set; }

  [Range(-100, 100, ErrorMessage = "temperature expected in human liveable conditions")]
  public required int MinimumTemperature { get; set; }

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
    return $"Day: {Day}, Maximum Temperature {MaximumTemperature}, Minimum Temperature: {MinimumTemperature}, Temperature Spread: {TemperatureSpread}";
  }
}


