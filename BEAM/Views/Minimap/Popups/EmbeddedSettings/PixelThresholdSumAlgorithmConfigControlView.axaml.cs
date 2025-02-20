
using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class PixelThresholdSumAlgorithmConfigControlView : SaveUserControl
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