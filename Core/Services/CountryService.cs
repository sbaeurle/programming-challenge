using BXCP.ProgrammingChallenge.Core.Models;
using BXCP.ProgrammingChallenge.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BXCP.ProgrammingChallenge.Core.Services;

internal class CountryService(IDictionary<string, ICountryReader> _readers, ILogger logger) : ICountryService
{
  public Result<Country> GetCountryWithHighestPopulationDensity(string source)
  {
    var ext = Path.GetExtension(source);

    logger.LogDebug("Calculate country with highest population density for {Source} with {Extension}", source, ext);

    if (!_readers.TryGetValue(ext, out var reader))
    {
      logger.LogError("No reader specified for file with {Extension}", ext);
      return Result.Fail($"no reader specified for file type {ext}");
    }

    var countriesResult = reader.ReadCountries(source);

    if (countriesResult.IsFailed)
    {
      logger.LogError("Reading countries failed");
      return Result.Fail(new Error($"could not extract countries from source {source}").CausedBy(countriesResult.Errors));
    }

    logger.LogInformation("Successfully retrieved countries from {Source}", source);

    var countries = countriesResult.Value;

    if (logger.IsEnabled(LogLevel.Debug))
    {
      logger.LogDebug("Retrieved {Count} countries from data source", countries.Count());
    }

    if (!countries.Any())
    {
      logger.LogError("Collection of countries is empty. Can't calculate highest population density");
      return Result.Fail("empty collection may yield no answer");
    }

    var result = countries.MaxBy(x => x.PopulationDensity);

    // if statement included for theoretical nullability check, however there is not realistic circumstance this could happen
    if (result is null)
    {
      logger.LogError("Calculation of highest population returned no value");
      return Result.Fail("calculation returned no value");
    }

    logger.LogInformation("Successfully calculated country with highest population");

    return Result.Ok(result);
  }
}
