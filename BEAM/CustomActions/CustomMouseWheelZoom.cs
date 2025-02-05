using System;
using ScottPlot;
using ScottPlot.Interactivity;

namespace BEAM.CustomActions;

public class CustomMouseWheelZoom(Key horizontalLockKey, Key verticalLockKey) : IUserActionResponse
{
    /// <summary>
    /// If enabled, when the mouse zooms while hovered over an axis only that axis will be changed.
    /// </summary>
    public bool ZoomAxisUnderMouse { get; set; } = true;

    /// <summary>
    /// If enabled with <see cref="ZoomAxisUnderMouse"/>, all axes of the same direction will be changed together.
    /// </summary>
    public bool LockParallelAxes { get; set; } = false;

    Key LockHorizontalKey { get; } = horizontalLockKey;

    Key LockVerticalKey { get; } = verticalLockKey;

    public void ResetState(Plot plot) { }

    /// <summary>
    /// Fraction of the axis range to change when zooming in and out.
    /// </summary>
    public double ZoomFraction { get; set; } = 0.15;
    
    public double ScrollFraction { get; set; } = 0.075;

    private double ZoomInFraction => 1 + ZoomFraction;
    private double ZoomOutFraction => 1 / ZoomInFraction;

    public ResponseInfo Execute(Plot plot, IUserAction userInput, KeyboardState keys)
    {
        if (keys.IsPressed(StandardKeys.Control))
        {
            if (userInput is ScottPlot.Interactivity.UserActions.MouseWheelUp mouseDownInput)
            {
                double xFrac = keys.IsPressed(LockHorizontalKey) ? 1 : ZoomInFraction;
                double yFrac = keys.IsPressed(LockVerticalKey) ? 1 : ZoomInFraction;
                MouseAxisManipulation.MouseWheelZoom(plot, xFrac, yFrac, mouseDownInput.Pixel, LockParallelAxes);
                return new ResponseInfo() { RefreshNeeded = true };
            }

            if (userInput is ScottPlot.Interactivity.UserActions.MouseWheelDown mouseUpInput)
            {
                double xFrac = keys.IsPressed(LockHorizontalKey) ? 1 : ZoomOutFraction;
                double yFrac = keys.IsPressed(LockVerticalKey) ? 1 : ZoomOutFraction;
                MouseAxisManipulation.MouseWheelZoom(plot, xFrac, yFrac, mouseUpInput.Pixel, LockParallelAxes);
                return new ResponseInfo() { RefreshNeeded = true };
            }

            return ResponseInfo.NoActionRequired;   
        }
        else
        {
            if (userInput is ScottPlot.Interactivity.UserActions.MouseWheelUp mouseDownInput)
            {
                var YSize = plot.Axes.GetLimits().Bottom - plot.Axes.GetLimits().Top;
                var pixel = new Pixel(mouseDownInput.Pixel.X, mouseDownInput.Pixel.Y + ScrollFraction * YSize);
                MouseAxisManipulation.DragPan(plot, mouseDownInput.Pixel, pixel);
                return new ResponseInfo() { RefreshNeeded = true };
            }
            
            if (userInput is ScottPlot.Interactivity.UserActions.MouseWheelDown mouseUpInput)
            {
                var YSize = plot.Axes.GetLimits().Bottom - plot.Axes.GetLimits().Top;
                var pixel = new Pixel(mouseUpInput.Pixel.X, mouseUpInput.Pixel.Y - ScrollFraction * YSize);
                MouseAxisManipulation.DragPan(plot, mouseUpInput.Pixel, pixel);
                return new ResponseInfo() { RefreshNeeded = true };
            }
            return ResponseInfo.NoActionRequired;
        }
    }
}
