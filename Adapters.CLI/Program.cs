using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Adapters.Cli;
using BXCP.ProgrammingChallenge.Adapters.Csv;
using BXCP.ProgrammingChallenge.Core.Services;
using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Services;

var rootCommand = new RootCommand(
  @"Weather and Country Source CLI
  Available Protocols:
        - file:// specify a source file on the local file system");

var fileSystem = new FileSystem();

var weatherSourceOption = new Option<string>(
    "--weather-source",
    "Specify the weather data source in the format protocol://source");
weatherSourceOption.AddValidator(Validator.ValidateArgument);

var countrySourceOption = new Option<string>(
    "--country-source",
    "Specify the country data source in the format protocol://source");
countrySourceOption.AddValidator(Validator.ValidateArgument);

rootCommand.Add(weatherSourceOption);
rootCommand.SetHandler(weatherSource =>
{
  var readers = new Dictionary<string, IWeatherReader>{
    {".csv", new CsvWeatherReader(fileSystem)}
  };

  IWeatherService weatherService = new WeatherService(readers);

  var result = weatherService.GetDayWithLeastTemperatureSpread(weatherSource);

  if (result.IsFailed)
  {
    Console.WriteLine($"Could not calculate day with least temperature spread: {result}");
    return;
  }

  var value = result.Value;
  Console.WriteLine($"Calculated day {value.Day} with minimum temperature spread of {value.TemperatureSpread} from source {weatherSource}");


}, weatherSourceOption);

return await rootCommand.InvokeAsync(args);