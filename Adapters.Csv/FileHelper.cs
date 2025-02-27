using System;

namespace BXCP.ProgrammingChallenge.Adapters.Csv;

internal static class FileHelper
{
  private const string FilePrefix = "file://";

  internal static string StripFileProtocol(string source)
  {
    if (!source.Contains(FilePrefix))
    {
      return source;
    }

    return source.Replace(FilePrefix, "");
  }
}
