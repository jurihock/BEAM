using System.Collections.Immutable;

namespace BEAM.Image.Minimap.Utility;

/// <summary>
/// A class responsible for transferring objects used for binding in views.
/// </summary>
/// <param name="allPossible">An immutable list of all possible objects (which can be selected).</param>
/// <param name="active">The currently active object or null.</param>
/// <typeparam name="T">The type of the objects being transferred.</typeparam>
public class SettingsTransferObject<T>(ImmutableList<T> allPossible, T? active)
{
    public T? Active { get; set; } = active;
    public readonly ImmutableList<T> AllPossible = allPossible;
}