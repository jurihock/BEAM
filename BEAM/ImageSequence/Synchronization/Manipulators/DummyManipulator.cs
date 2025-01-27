using ScottPlot.Avalonia;

namespace BEAM.ImageSequence.Synchronization.Manipulators;

public class DummyManipulator : Manipulator
{
    public override bool SyncPlot(AvaPlot avaPlot)
    {
        return true;
    }

    public override bool UnsyncPlot(AvaPlot avaPlot)
    {
        return true;
    }
}