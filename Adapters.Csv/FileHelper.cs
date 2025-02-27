using System.IO.Abstractions;

namespace BXCP.ProgrammingChallenge.Adapters.Csv;

internal static class FileHelper
{
  private const string FilePrefix = "file://";
  private static readonly char[] possibleDelimiters = [',', ';', '\t', '|'];

  internal static string StripFileProtocol(string source)
  {
    if (!source.Contains(FilePrefix))
    {
      return source;
    }

    return source.Replace(FilePrefix, "");
  }

  internal static string DetectCsvDelimiter(IFileSystem fileSystem, string filePath)
  {

    using var fileStream = fileSystem.File.OpenRead(filePath);
    using var reader = new StreamReader(fileStream);
    var firstLine = reader.ReadLine();

    if (string.IsNullOrEmpty(firstLine))
    {
      return ",";
    }

    var delimiterCounts = new Dictionary<char, int>();
    foreach (var delimiter in possibleDelimiters)
    {
      delimiterCounts[delimiter] = firstLine.Count(c => c == delimiter);
    }

    // Return the delimiter with the highest count
    return delimiterCounts.OrderByDescending(kv => kv.Value).First().Key.ToString();
  }
}
