using System;
using ScottPlot;
using ScottPlot.Interactivity;
using ScottPlot.Plottables;

namespace BEAM.CustomActions;

/// <summary>
/// Class used to implement the region selection for analysis.
/// Simply draws a rectangle for the selected area, without executing additional functionality.
///
/// Inspired by:
/// https://github.com/ScottPlot/ScottPlot/blob/main/src/ScottPlot5/ScottPlot5 Demos/ScottPlot5 WinForms Demo/Demos/SelectPoints.cs#L89
/// </summary>

public class CustomAreaSelection(MouseButton button) : IUserActionResponse
{
    Coordinates MouseDownCoordinates = Coordinates.NaN;
    private CoordinateRect MouseSelectionRect = CoordinateRect.Empty;
    bool MouseIsDown = false;
    ScottPlot.Plottables.Rectangle SelectionRectangle = new Rectangle();

    /// <summary>
    /// A selection rectangle is started when this button is pressed and dragged
    /// </summary>
    MouseButton MouseButton { get; set; } = button;
    
    public void ResetState(Plot plot)
    {
        MouseDownCoordinates = Coordinates.NaN;
        MouseSelectionRect = new CoordinateRect();
        SelectionRectangle = plot.Add.Rectangle(0, 0, 0, 0);
    }


    public ResponseInfo Execute(Plot plot, IUserAction userAction, KeyboardState keys)
    {
        // If the button is first pressed --> Add rectangle and fix first coordinate
        if (userAction is IMouseButtonAction buttonAction && buttonAction.IsPressed && buttonAction.Button == MouseButton)
        {
            MouseIsDown = true;
            SelectionRectangle = plot.Add.Rectangle(0, 0, 0, 0);
            //SelectionRectangle.FillStyle.Color = Colors.Red.WithAlpha(0.4); // Set default color: Red (otherwise random)
            SelectionRectangle.IsVisible = true;
            plot.MoveToTop(SelectionRectangle);
            MouseDownCoordinates = plot.GetCoordinates(buttonAction.Pixel.X, buttonAction.Pixel.Y);
        }
        // If the targeted position is not in the Graph?
        if (MouseDownCoordinates == Coordinates.NaN)
        {
            return ResponseInfo.NoActionRequired;
        }
        // If the mouse is moved, adapt the rectangle
        if (userAction is IMouseAction mouseMoveAction && userAction is not IMouseButtonAction && MouseIsDown)
        {
            SelectionRectangle.IsVisible = true;
            Coordinates mouseNowCoordinates = 
                plot.GetCoordinates(mouseMoveAction.Pixel.X, mouseMoveAction.Pixel.Y);

            MouseSelectionRect = new CoordinateRect(MouseDownCoordinates, mouseNowCoordinates);
            SelectionRectangle.CoordinateRect = MouseSelectionRect;

            return ResponseInfo.Refresh;
        }
        
        if (userAction is IMouseButtonAction mouseUpAction && !mouseUpAction.IsPressed && mouseUpAction.Button == MouseButton)
        {
            MouseDownCoordinates = Coordinates.NaN;
            
            if (SelectionRectangle.IsVisible)
            {
                SelectionRectangle.IsVisible = false;
                return ResponseInfo.Refresh;
            }
            MouseIsDown = false;
        }

        return ResponseInfo.NoActionRequired;
    }
}