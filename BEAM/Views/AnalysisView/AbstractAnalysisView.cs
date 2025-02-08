using Avalonia.Controls;
using BEAM.Analysis;
using BEAM.Datatypes;
using BEAM.ImageSequence;
using ScottPlot;

namespace BEAM.Views.AnalysisView;

/// <summary>
/// Abstract parent class of analysis Views. Used to create concrete subclass instances
/// via factory method.
/// </summary>
public abstract class AbstractAnalysisView : UserControl
{
    public abstract void Update(IPlottable plottable);
}