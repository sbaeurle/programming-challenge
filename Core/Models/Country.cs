namespace BXCP.ProgrammingChallenge.Core.Models;

public class Country
{
  public required string Name { get; set; }

  public required double Population { get; set; }

  public required double Area { get; set; }

  public double PopulationDensity
  {
    get
    {
      if (Math.Abs(Area) < 0.001)
      {
        return double.NaN;
      }

      return Population / Area;
    }
  }

  public override string ToString()
  {
    return $"Country: {Name}, Population: {Population}, Area: {Area}, Population Density: {PopulationDensity}";
  }
}