using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels.AnalysisViewModels;

public abstract class AnalysisViewModelPlot : ViewModelBase
{
    public abstract void Update();
}