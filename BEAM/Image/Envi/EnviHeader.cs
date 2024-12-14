using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BEAM.Image.Envi;

public sealed class EnviHeader
{
  private readonly string Text;
  private readonly Dictionary<string, string> Values;

  public EnviHeader(string filepath)
  {
    Text = File.ReadAllText(filepath);

    Values = new Dictionary<string, string>(Text
      .Split(Environment.NewLine)
      .Select(line => line.Trim())
      .Where(line => !string.IsNullOrEmpty(line))
      .Select(line => line.Split(['='], 2))
      .Where(line => line.Length == 2)
      .Select(line => KeyValuePair.Create(
        line.First().Trim(),
        line.Last().Trim())),
      StringComparer.OrdinalIgnoreCase);
  }

  public TValue Get<TValue>(string key, object? def = null)
  {
    var value = Values.GetValueOrDefault(key) ?? def?.ToString();

    if (value is null)
    {
      throw new KeyNotFoundException(
        $"Missing ENVI header field \"{key}\"!");
    }
    else if (typeof(TValue).IsEnum)
    {
      return (TValue)Enum.Parse(
        typeof(TValue), value, ignoreCase: true);
    }
    else
    {
      return (TValue)Convert.ChangeType(
        value, typeof(TValue));
    }
  }

  public override string ToString()
  {
    return Text;
  }
}
