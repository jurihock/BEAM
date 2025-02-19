using System.Collections.Immutable;

namespace BEAM.Image.Minimap.Utility;

public interface ISettingsProvider<TResult>
{
    public ImmutableList<TResult> GetDefaultObjects();

    public void SetDefaultObject(TResult? newAlgorithmDefault);

    public bool ExistAny();
    public TResult? GetDefaultObject();

    public SettingsTransferObject<TResult> GetDefaultClones();

}