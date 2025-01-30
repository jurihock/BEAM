using Avalonia.Controls;

namespace BEAM.Views.AnalysisView;

/// <summary>
/// Abstract parent class of analysis Views. Used to create concrete subclass instances
/// via factory method.
/// </summary>
public abstract class AbstractAnalysisView : UserControl
{
    public abstract void Update(double[] newData);
}