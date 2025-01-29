using System;
using System.Collections.Generic;
using System.Linq;
using ScottPlot.Avalonia;

namespace BEAM.ImageSequence.Synchronization.Manipulators;

public class MouseManipulator : Manipulator
{
    
    private List<AvaPlot> _avaPlots = [];
    
    public override bool SyncPlot(AvaPlot avaPlot)
    {
        _avaPlots.Add(avaPlot);
        Console.WriteLine("Added Plot to Manipulator");
        
        avaPlot.PointerEntered += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            Console.WriteLine("Fired!");
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.PointerExited += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.PointerMoved += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.PointerPressed += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.PointerReleased += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.PointerWheelChanged += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.PointerCaptureLost += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.Tapped += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.DoubleTapped += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        avaPlot.Holding += (s, e) =>
        {
            if (e.Handled)
            {
                return;
            }

            e.Handled = true;
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.RaiseEvent(e);
            }
        };
        
        return true;
    }

    public override bool UnsyncPlot(AvaPlot avaPlot)
    {
        return true;
    }
}