using BXCP.ProgrammingChallenge.Core.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Core.Services;

internal class CountryService(IDictionary<string, ICountryReader> _readers) : ICountryService
{
  public Result<Country> GetCountryWithHighestPopulationDensity(string source)
  {
    var ext = Path.GetExtension(source);

    if (!_readers.TryGetValue(ext, out var reader))
    {
      return Result.Fail($"no reader specified for file type {ext}");
    }

    var countriesResult = reader.ReadCountries(source);

    if (countriesResult.IsFailed)
    {
      return Result.Fail(new Error($"could not extract countries from source {source}").CausedBy(countriesResult.Errors));
    }

    var countries = countriesResult.Value;

    if (!countries.Any())
    {
      return Result.Fail("empty collection may yield no answer");
    }

    var result = countries.MaxBy(x => x.PopulationDensity);

    // if statement included for theoretical nullability check, however there is not realistic circumstance this could happen
    if (result is null)
    {
      return Result.Fail("calculation returned no value");
    }

    return Result.Ok(result);
  }
}
