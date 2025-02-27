using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;
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

var weatherCommand = new Command("weather", "calculate temperature spread");

var weatherSourceOption = new Option<string>(
    "--weather-source",
    "Specify the weather data source in the format protocol://source")
{
  IsRequired = true
};
weatherSourceOption.AddValidator(Validator.ValidateArgument);

weatherCommand.AddOption(weatherSourceOption);
weatherCommand.SetHandler(weatherSource =>
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

  var weatherRecord = result.Value;
  Console.WriteLine($"Calculated day {weatherRecord.Day} with minimum temperature spread of {weatherRecord.TemperatureSpread} from source {weatherSource}");
}, weatherSourceOption);

rootCommand.AddCommand(weatherCommand);

var countryCommand = new Command("country", "calculate population density");

var countrySourceOption = new Option<string>(
    "--country-source",
    "Specify the country data source in the format protocol://source")
{
  IsRequired = true
};
countrySourceOption.AddValidator(Validator.ValidateArgument);

countryCommand.AddOption(countrySourceOption);
countryCommand.SetHandler(countrySource =>
{
  var readers = new Dictionary<string, ICountryReader>{
    {".csv", new CsvCountryReader(fileSystem)}
  };

  ICountryService countryService = new CountryService(readers);

  var result = countryService.GetCountryWithHighestPopulationDensity(countrySource);

  if (result.IsFailed)
  {
    Console.WriteLine($"Could not calculate country with highest population density: {result}");
    return;
  }

  var country = result.Value;
  Console.WriteLine($"Calculated country {country.Name} with highest population density of {country.PopulationDensity.ToString("F2", CultureInfo.CurrentCulture)} per km² from source {countrySource}");
}, countrySourceOption);

rootCommand.AddCommand(countryCommand);

return await rootCommand.InvokeAsync(args);