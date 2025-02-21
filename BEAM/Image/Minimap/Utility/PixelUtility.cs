using BEAM.Datatypes.Color;

namespace BEAM.Image.Minimap.Utility;

/// <summary>
/// This class provides utility methods for the <see cref="BGR"/> struct.
/// </summary>
public static class PixelUtility
{
    /// <summary>
    /// Checks whether this instance's channel values are all greater than or equal to another instances channel values.
    /// </summary>
    /// <param name="baseline">The BGR instance which is meant to be compared as a base.</param>
    /// <param name="other">The instance to compare against.</param>
    /// <returns>True if instance's channel values are all greater than or equal to other's channel values.</returns>
    public static bool EntrywiseAllGreaterEqual(this BGR baseline, BGR other)
    {

        return (baseline.R >= other.R) && (baseline.G >= other.G) && (baseline.B >= other.B);
    }

    /// <summary>
    /// Checks whether this instance's has a channel whose value is greater than another instance's value of the same channel.</summary>
    /// <param name="baseline">The BGR instance which is meant to be compared as a base.</param>
    /// <param name="other">The instance to compare against.</param>
    /// <returns>True if instance has a channel whose value is greater than other's value of the same channel.</returns>
    public static bool EntrywiseAnyGreater(this BGR baseline, BGR other)
    {
        return (baseline.R > other.R) || (baseline.G > other.G) || (baseline.B > other.B);
    }
}