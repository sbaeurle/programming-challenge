using CsvHelper.Configuration.Attributes;

namespace BXCP.ProgrammingChallenge.Adapters.Csv.Models;

public class Country
{
  [Name("Name")]
  public required string Name { get; set; }

  [Name("Population")]
  public required double Population { get; set; }

  [Name("Area (km²)")]
  public required double Area { get; set; }
}