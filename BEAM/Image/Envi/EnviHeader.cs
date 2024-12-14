using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BEAM.Image.Envi;

/// <summary>
/// This class represents the contents of an ENVI header file and therefore contains metadata about the raw ENVI file.
/// </summary>
public sealed class EnviHeader
{
  private readonly string Text;
  private readonly Dictionary<string, string> Values;

  /// <summary>
  /// Creates a new instance from the ENVI header file.
  /// </summary>
  /// <param name="filepath">The path of an ENVI header file whose contents are meant to be realised in the new instance.</param>
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

  /// <summary>
  /// Gets an attribute/setting given its name as a string
  /// </summary>
  /// <param name="key">The setting's name as a string.</param>
  /// <param name="def">A default object which will be used if no settings entry with the provided name was fund. Default is null.</param>
  /// <typeparam name="TValue">The type of the value which corresponds to this setting.</typeparam>
  /// <returns>The value which corresponds to the setting with the strings name.</returns>
  /// <exception cref="KeyNotFoundException">If no setting with this name could be found.</exception>
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
