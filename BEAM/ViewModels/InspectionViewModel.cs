using BEAM.Docking;

namespace BEAM.ViewModels;

public class InspectionViewModel : ViewModelBase, IDockBase
{
    public string Name { get; } = "Inspect";
}