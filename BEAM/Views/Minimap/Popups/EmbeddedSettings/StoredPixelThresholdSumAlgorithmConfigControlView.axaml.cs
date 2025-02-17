using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ViewModels.Minimap.Popups;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class StoredPixelThresholdSumAlgorithmConfigControlView : ISaveControl
{
    public StoredPixelThresholdSumAlgorithmConfigControlView(RenderedPixelThresholdAlgorithm algorithm, SettingsStorer storer)
    {
        InitializeComponent();
        DataContext = new StoredPixelThresholdSumAlgorithmConfigControlViewModel(algorithm, storer);
    }

    public override void Save()
    {
        (DataContext as PixelThresholdSumAlgorithmConfigControlViewModel)!.Save();
    }
}