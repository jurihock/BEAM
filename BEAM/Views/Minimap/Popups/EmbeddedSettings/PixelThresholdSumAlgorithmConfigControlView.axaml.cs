using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ViewModels.Minimap.Popups;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class PixelThresholdSumAlgorithmConfigControlView : ISaveControl
{
    public PixelThresholdSumAlgorithmConfigControlView(RenderedPixelThresholdAlgorithm algorithm)
    {
        InitializeComponent();
        DataContext = new PixelThresholdSumAlgorithmConfigControlViewModel(algorithm);
    }

    public override void Save()
    {
        (DataContext as PixelThresholdSumAlgorithmConfigControlViewModel)!.Save();
    }
}