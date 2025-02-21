using BEAM.Image.Minimap.MinimapAlgorithms;
using BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;
using BEAM.Views.Utility;

namespace BEAM.Views.Minimap.Popups.EmbeddedSettings;

public partial class PixelThresholdAnySumAlgorithmConfigControlView : SaveUserControl
{
    public PixelThresholdAnySumAlgorithmConfigControlView(RenderedPixelAnyThresholdAlgorithm algorithm)
    {
        InitializeComponent();
        DataContext = new PixelThresholdAnySumAlgorithmConfigControlViewModel(algorithm);
    }

    public override void Save()
    {
        (DataContext as PixelThresholdAnySumAlgorithmConfigControlViewModel)!.Save();
    }
}