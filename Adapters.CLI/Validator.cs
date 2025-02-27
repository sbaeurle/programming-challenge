using System;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;
using CsvHelper.Configuration.Attributes;

namespace BXCP.ProgrammingChallenge.Adapters.Cli;

public static class Validator
{
  static internal readonly string[] allowedExtensions = [
    ".csv"
  ];

  public static void ValidateArgument(OptionResult result)
  {
    var value = result.GetValueOrDefault<string>();

    if (value != null && !Regex.IsMatch(value, @"^file://.+"))
    {
      var ext = Path.GetExtension(value);

      if (!allowedExtensions.Contains(ext))
      {
        result.ErrorMessage = $"Invalid file format. Allowed values: {string.Join(", ", allowedExtensions)}";
      }
    }
  }
}
