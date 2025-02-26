using System.IO.Abstractions;
using BXCP.ProgrammingChallenge.Adapters.Csv;
using BXCP.ProgrammingChallenge.Core;
using BXCP.ProgrammingChallenge.Interfaces;

var fileSystem = new FileSystem();
IWeatherReader weatherReader = new CsvWeatherReader(fileSystem);
IWeatherService weatherService = new WeatherService();

var recordsResult = weatherReader.ReadWeatherRecords("/home/sbaeurle/projects/programming-challenge/weather.csv");

if (recordsResult.IsFailed)
{
  return;
}

var dayResult = weatherService.GetDayWithLeastTemperatureSpread(recordsResult.Value);

if (dayResult.IsFailed)
{
  return;
}

Console.WriteLine(dayResult.Value);