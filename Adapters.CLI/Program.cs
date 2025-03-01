using System.CommandLine;
using System.CommandLine.Parsing;
using System.Globalization;
using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Adapters.Cli;
using BXCP.ProgrammingChallenge.Adapters.Csv;
using BXCP.ProgrammingChallenge.Core.Services;
using BXCP.ProgrammingChallenge.Interfaces;
using BXCP.ProgrammingChallenge.Services;
using FluentResults;
using Microsoft.Extensions.Logging;
using Serilog;

var rootCommand = new RootCommand(
  @"Weather and Country Source CLI
  Available Protocols:
        - file:// specify a source file on the local file system");

var fileSystem = new FileSystem();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(Log.Logger));
var logger = loggerFactory.CreateLogger<Program>();

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
    {".csv", new CsvWeatherReader(fileSystem, loggerFactory.CreateLogger<CsvWeatherReader>())}
  };

  IWeatherService weatherService = new WeatherService(readers, loggerFactory.CreateLogger<WeatherService>());

  var result = weatherService.GetDayWithLeastTemperatureSpread(weatherSource);

  if (result.IsFailed)
  {
    logger.LogError("Could not calculate day with least temperature spread: {@Reason}", result.Errors[result.Errors.Count - 1]);
    return;
  }

  var weatherRecord = result.Value;
  logger.LogInformation("Calculated day {Day} with minimum temperature spread of {TemperatureSpread} from source {WeatherSource}", weatherRecord.Day, weatherRecord.TemperatureSpread, weatherSource);
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
    {".csv", new CsvCountryReader(fileSystem, loggerFactory.CreateLogger<CsvCountryReader>())}
  };

  ICountryService countryService = new CountryService(readers, loggerFactory.CreateLogger<CountryService>());

  var result = countryService.GetCountryWithHighestPopulationDensity(countrySource);

  if (result.IsFailed)
  {
    logger.LogError("Could not calculate country with highest population density: {@Reason}", result.Errors[result.Errors.Count - 1]);
    return;
  }

  var country = result.Value;
  logger.LogInformation("Calculated country {Name} with highest population density of {PopulationDensity} per km² from source {CountrySource}", country.Name, country.PopulationDensity.ToString("F2", CultureInfo.CurrentCulture), countrySource);
}, countrySourceOption);

rootCommand.AddCommand(countryCommand);

return await rootCommand.InvokeAsync(args);