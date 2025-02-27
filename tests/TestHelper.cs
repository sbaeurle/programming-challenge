using System.Reflection;
using System.Text;

namespace BXCP.ProgrammingChallenge.Tests;

internal static class TestHelper
{
  internal static string ReadEmbeddedResource(string resourceName)
  {
    var assembly = Assembly.GetExecutingAssembly();
    using var stream = assembly.GetManifestResourceStream(resourceName);

    if (stream == null)
    {
      throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");
    }

    using var reader = new StreamReader(stream, Encoding.UTF8);
    return reader.ReadToEnd();
  }
}
