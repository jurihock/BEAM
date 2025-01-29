using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Interactivity;
using BEAM.Log;
using ScottPlot.Avalonia;

namespace BEAM.ImageSequence.Synchronization.Manipulators;

public class MouseManipulator : Manipulator
{
    
    private List<AvaPlot> _avaPlots = [];
    
    public override bool SyncPlot(AvaPlot avaPlot)
    {
        _avaPlots.Add(avaPlot);
        
        avaPlot.PointerEntered += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.PointerExited += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.PointerMoved += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.PointerPressed += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.PointerReleased += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.PointerWheelChanged += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.Tapped += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.DoubleTapped += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };

        avaPlot.Holding += (s, e) =>
        {
            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                }
            }
        };
            
        return true;
    }

    public override bool UnsyncPlot(AvaPlot avaPlot)
    {
        return true;
    }
}