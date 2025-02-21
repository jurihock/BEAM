using BEAM.Image.Minimap.MinimapAlgorithms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

public partial class PixelThresholdAnySumAlgorithmConfigControlViewModel : ViewModelBase
{
    [ObservableProperty] public partial byte SelectedRedThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedGreenThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedBlueThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedAlphaThreshold { get; set; }
    
    private readonly RenderedPixelAnyThresholdAlgorithm _algorithm;
    public PixelThresholdAnySumAlgorithmConfigControlViewModel(RenderedPixelAnyThresholdAlgorithm algorithm)
    {
        _algorithm = algorithm;
        SelectedRedThreshold = algorithm.ThresholdRed;
        SelectedGreenThreshold = algorithm.ThresholdGreen;
        SelectedBlueThreshold = algorithm.ThresholdBlue;
        SelectedAlphaThreshold = algorithm.ThresholdAlpha;
    }
    public void Save()
    {
        _algorithm.ThresholdRed = SelectedRedThreshold;
        _algorithm.ThresholdGreen = SelectedGreenThreshold;
        _algorithm.ThresholdBlue = SelectedBlueThreshold;
        _algorithm.ThresholdAlpha = SelectedAlphaThreshold;
    }
}