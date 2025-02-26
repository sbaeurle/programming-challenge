using System;
using System.Globalization;
using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Models;
using CsvHelper;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Adapters.Csv;

public class CsvWeatherReader(IFileSystem fileSystem) : IWeatherReader
{
  public Result<IEnumerable<WeatherRecord>> ReadWeatherRecords(string filepath)
  {
    if (!fileSystem.File.Exists(filepath))
    {
      return Result.Fail(new Error($"file not found at specified path: {filepath}"));
    }

    if (fileSystem.Path.GetExtension(filepath) != ".csv")
    {
      return Result.Fail("file uses incompatible extension for csv");
    }

    using var fileStream = fileSystem.File.OpenRead(filepath);
    using var reader = new StreamReader(fileStream);
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

    try
    {
      var records = csv.GetRecords<WeatherRecord>().ToList(); // ToList is necessary to materialize the list

      return Result.Ok<IEnumerable<WeatherRecord>>(records);
    }
    catch (CsvHelperException ex)
    {
      return Result.Fail(new Error("could not parse csv file into weather record object").CausedBy(ex));
    }
  }
}
