using BEAM.Image.Minimap.MinimapAlgorithms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

public partial class PixelThresholdSumAlgorithmConfigControlViewModel : ViewModelBase
{
    [ObservableProperty] public partial byte SelectedRedThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedGreenThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedBlueThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedGammaThreshold { get; set; }
    
    private readonly RenderedPixelThresholdAlgorithm _algorithm;
    public PixelThresholdSumAlgorithmConfigControlViewModel(RenderedPixelThresholdAlgorithm algorithm)
    {
        _algorithm = algorithm;
        SelectedRedThreshold = algorithm.ThresholdRed;
        SelectedGreenThreshold = algorithm.ThresholdGreen;
        SelectedBlueThreshold = algorithm.ThresholdBlue;
        SelectedGammaThreshold = algorithm.ThresholdGamma;
    }
    public void Save()
    {
        _algorithm.ThresholdRed = SelectedRedThreshold;
        _algorithm.ThresholdGreen = SelectedGreenThreshold;
        _algorithm.ThresholdBlue = SelectedBlueThreshold;
        _algorithm.ThresholdGamma = SelectedGammaThreshold;
    }
}