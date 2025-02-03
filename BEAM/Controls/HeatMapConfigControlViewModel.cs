// (c) Paul Stier, 2025

using BEAM.Renderer;
using BEAM.ViewModels;

namespace BEAM.Controls;

public class HeatMapConfigControlViewModel : ViewModelBase
{
    public HeatMapRenderer Renderer { get; set; }
    public HeatMapConfigControlViewModel(HeatMapRenderer renderer)
    {
        Renderer = renderer;
    }
}