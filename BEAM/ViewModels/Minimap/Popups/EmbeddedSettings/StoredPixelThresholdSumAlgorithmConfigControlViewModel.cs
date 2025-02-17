using System;
using BEAM.Image.Minimap;
using BEAM.Image.Minimap.MinimapAlgorithms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.Minimap.Popups.EmbeddedSettings;

public partial class StoredPixelThresholdSumAlgorithmConfigControlViewModel: ViewModelBase
{
    [ObservableProperty] public partial byte SelectedRedThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedGreenThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedBlueThreshold { get; set; }
    [ObservableProperty] public partial byte SelectedGammaThreshold { get; set; }
    
    private readonly RenderedPixelThresholdAlgorithm _algorithm;
    private readonly SettingsStorer _storer;
    public StoredPixelThresholdSumAlgorithmConfigControlViewModel(RenderedPixelThresholdAlgorithm algorithm, SettingsStorer storer)
    {
        _storer = storer;
        _algorithm = algorithm;
        SelectedRedThreshold = algorithm.ThresholdRed;
        SelectedGreenThreshold = algorithm.ThresholdGreen;
        SelectedBlueThreshold = algorithm.ThresholdBlue;
        SelectedGammaThreshold = algorithm.ThresholdGamma;
    }
    public void Save()
    {
        Console.WriteLine("Is algo null?: " );
        Console.WriteLine(_algorithm is null);
        _algorithm.ThresholdRed = SelectedRedThreshold;
        _algorithm.ThresholdGreen = SelectedGreenThreshold;
        _algorithm.ThresholdBlue = SelectedBlueThreshold;
        _algorithm.ThresholdGamma = SelectedGammaThreshold;
    }
}