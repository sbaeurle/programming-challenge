using System.Globalization;
using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Adapters.Csv.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using CsvHelper;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Adapters.Csv;

public class CsvWeatherReader(IFileSystem fileSystem) : IWeatherReader
{
  public Result<IEnumerable<Core.Models.WeatherRecord>> ReadWeatherRecords(string source)
  {
    source = FileHelper.StripFileProtocol(source);

    if (!fileSystem.File.Exists(source))
    {
      return Result.Fail(new Error($"file not found at specified path: {source}"));
    }

    if (fileSystem.Path.GetExtension(source) != ".csv")
    {
      return Result.Fail("file uses incompatible extension for csv");
    }

    using var fileStream = fileSystem.File.OpenRead(source);
    using var reader = new StreamReader(fileStream);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    try
    {
      var records = csv.GetRecords<WeatherRecord>().ToList(); // ToList is necessary to materialize the list

      return Result.Ok(records.Select(x => new Core.Models.WeatherRecord
      {
        Day = x.Day,
        MaximumTemperature = x.MaximumTemperature,
        MinimumTemperature = x.MinimumTemperature
      }));
    }
    catch (CsvHelperException ex)
    {
      return Result.Fail(new Error("could not parse csv file into weather record object").CausedBy(ex));
    }
  }
}
