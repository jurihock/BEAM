using System.Collections.Immutable;

namespace BEAM.Image.Minimap.Utility;

public class SettingsTransferObject<T>(ImmutableList<T> allPossible, T? active)
{
    public T? Active { get; set; } = active;
    public readonly ImmutableList<T> AllPossible = allPossible;
}