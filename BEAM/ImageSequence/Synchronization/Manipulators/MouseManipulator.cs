using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using ScottPlot;
using ScottPlot.Avalonia;

namespace BEAM.ImageSequence.Synchronization.Manipulators;

/// <summary>
/// A class used to share mouse events between a group of Avaplots.
/// </summary>
public class MouseManipulator : Manipulator
{
    /// <summary>
    /// The list of the plots, which will share mouse events.
    /// </summary>
    private readonly List<AvaPlot> _avaPlots = [];

    private bool _isSynchronizing;

    /// <summary>
    /// Synchronizes a plot with all the other plots, which are already registered.
    /// </summary>
    /// <param name="avaPlot">The AvaPlot, which will be synchronized.</param>
    /// <returns>A Boolean, indicating, if the given AvaPlot was successfully synchronized.</returns>
    public override bool SyncPlot(AvaPlot? avaPlot)
    {
        if (avaPlot == null)
        {
            return false;
        }

        _avaPlots.Add(avaPlot);

        void OnAvaPlotOnPointerEntered(object? _, PointerEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        avaPlot.PointerEntered += OnAvaPlotOnPointerEntered;

        void OnAvaPlotOnPointerExited(object? _, PointerEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        avaPlot.PointerExited += OnAvaPlotOnPointerExited;

        void OnAvaPlotOnPointerMoved(object? _, PointerEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        avaPlot.PointerMoved += OnAvaPlotOnPointerMoved;

        void OnAvaPlotOnPointerPressed(object? _, PointerPressedEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            // check for right click -> only opens context push menu (Scottplot internal)
            if (e.GetCurrentPoint(avaPlot).Properties.IsRightButtonPressed)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        avaPlot.PointerPressed += OnAvaPlotOnPointerPressed;

        void OnAvaPlotOnPointerReleased(object? _, PointerReleasedEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        avaPlot.PointerReleased += OnAvaPlotOnPointerReleased;

        void OnAvaPlotOnPointerWheelChanged(object? _, PointerWheelEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));
            foreach (var plot in _avaPlots.Where(p => p != avaPlot))
            {
                plot.Plot.Axes.SetLimits(avaPlot.Plot.Axes.GetLimits());
                plot.Refresh();
                ScrollingSynchronizerMapper.UpdateOwnScrollBar(plot);
                ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
            }
        }

        avaPlot.PointerWheelChanged += OnAvaPlotOnPointerWheelChanged;

        void OnAvaPlotOnTapped(object? _, TappedEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        avaPlot.Tapped += OnAvaPlotOnTapped;

        void OnAvaPlotOnDoubleTapped(object? _, TappedEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                var coordinates = avaPlot.Plot.GetCoordinates(new Pixel(e.GetPosition(avaPlot).X, e.GetPosition(avaPlot).Y));

                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    ScrollingSynchronizerMapper.GetSequenceView(plot).UpdatePositionAnnotation(coordinates.X, coordinates.Y);
                }
            }
        }

        
        avaPlot.DoubleTapped += OnAvaPlotOnDoubleTapped;

        void OnAvaPlotOnHolding(object? _, HoldingRoutedEventArgs e)
        {
            if (!_isSynchronizing)
            {
                return;
            }

            EventSourceMapper.AddIfNotExists(e, avaPlot);
            if (EventSourceMapper.IsSource(e, avaPlot))
            {
                foreach (var plot in _avaPlots.Where(p => p != avaPlot))
                {
                    plot.RaiseEvent(e);
                    //ScrollingSynchronizer.UpdateOwnScrollBar(plot);
                }
            }
        }

        avaPlot.Holding += OnAvaPlotOnHolding;

        return true;
    }

    /// <summary>
    /// Removes an AvaPlot, so that it is no longer synchronized with the other plots.
    /// </summary>
    /// <param name="avaPlot">The AvaPlot, which synchronization will be removed.</param>
    /// <returns>A Boolean indicating, if the plot was removed successfully.</returns>
    public override bool UnsyncPlot(AvaPlot? avaPlot)
    {
        return avaPlot != null && _avaPlots.Remove(avaPlot);
    }

    /// <summary>
    /// This method is used to activate the synchronization between all plots.
    /// </summary>
    public override void Activate()
    {
        _isSynchronizing = true;
    }

    /// <summary>
    /// This method is used to deactivate the synchronization between all plots.
    /// </summary>
    public override void Deactivate()
    {
        _isSynchronizing = false;
    }
}