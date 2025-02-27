using System.Globalization;
using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Adapters.Csv.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Adapters.Csv;

public class CsvCountryReader(IFileSystem _fileSystem) : ICountryReader
{
  public Result<IEnumerable<Core.Models.Country>> ReadCountries(string source)
  {
    source = FileHelper.StripFileProtocol(source);

    if (!_fileSystem.File.Exists(source))
    {
      return Result.Fail(new Error($"file not found at specified path: {source}"));
    }

    if (_fileSystem.Path.GetExtension(source) != ".csv")
    {
      return Result.Fail("file uses incompatible extension for csv");
    }

    var delimiter = FileHelper.DetectCsvDelimiter(_fileSystem, source);
    var options = new CsvConfiguration(new CultureInfo("de-DE"))
    {
      Delimiter = delimiter,
    };

    using var fileStream = _fileSystem.File.OpenRead(source);
    using var reader = new StreamReader(fileStream);
    using var csv = new CsvReader(reader, options);

    try
    {
      var records = csv.GetRecords<Country>().ToList(); // ToList is necessary to materialize the list

      return Result.Ok(records.Select(x => new Core.Models.Country
      {
        Name = x.Name,
        Area = x.Area,
        Population = x.Population
      }));
    }
    catch (CsvHelperException ex)
    {
      return Result.Fail(new Error("could not parse csv file into weather record object").CausedBy(ex));
    }
  }
}
