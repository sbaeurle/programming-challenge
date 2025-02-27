using BXCP.ProgrammingChallenge.Core.Models;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Core.Services;

public interface ICountryService
{
  public Result<Country> GetCountryWithHighestPopulationDensity(string source);
}
