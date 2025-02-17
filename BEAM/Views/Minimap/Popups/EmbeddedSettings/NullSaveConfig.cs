using Avalonia.Input.TextInput;
using BEAM.ViewModels.Minimap.Popups;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public class NullSaveConfig : ISaveControl
{
    public override void Save()
    {
        return;
    }
}