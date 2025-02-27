using BXCP.ProgrammingChallenge.Core.Models;
using FluentResults;

namespace BXCP.ProgrammingChallenge.Interfaces;

public interface ICountryReader
{
  public Result<IEnumerable<Country>> ReadCountries(string source);
}
