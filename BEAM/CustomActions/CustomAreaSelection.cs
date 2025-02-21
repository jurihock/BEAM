using Avalonia;
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
    private Coordinates _mouseDownCoordinates = Coordinates.NaN;
    private CoordinateRect _mouseSelectionRect = CoordinateRect.Empty;
    private bool _mouseIsDown;
    private Rectangle _selectionRectangle = new Rectangle();

    /// <summary>
    /// A selection rectangle is started when this button is pressed and dragged
    /// </summary>
    MouseButton MouseButton { get; set; } = button;
    
    public void ResetState(Plot plot)
    {
        _mouseDownCoordinates = Coordinates.NaN;
        _mouseSelectionRect = new CoordinateRect();
        _selectionRectangle = plot.Add.Rectangle(0, 0, 0, 0);
    }


    public ResponseInfo Execute(Plot plot, IUserAction userAction, KeyboardState keys)
    {
        // If the button is first pressed --> Add rectangle and fix first coordinate
        if (userAction is IMouseButtonAction { IsPressed: true } buttonAction && buttonAction.Button == MouseButton)
        {
            var currentTheme = Application.Current!.ActualThemeVariant;
            
            Application.Current.TryGetResource("OverlayColor", currentTheme, out var overlayColor);
            var colorAvalonia = (Avalonia.Media.Color)overlayColor;
            var colorScottPlot = new Color(colorAvalonia.R, colorAvalonia.G, colorAvalonia.B);
            
            _mouseIsDown = true;
            _selectionRectangle = plot.Add.Rectangle(0, 0, 0, 0);
            
            _selectionRectangle.LineColor = colorScottPlot;
            _selectionRectangle.FillStyle.Color = colorScottPlot.WithAlpha(0.4); // Set default color: Red (otherwise random)
            _selectionRectangle.IsVisible = true;
            plot.MoveToTop(_selectionRectangle);
            _mouseDownCoordinates = plot.GetCoordinates(buttonAction.Pixel.X, buttonAction.Pixel.Y);
        }
        // If the targeted position is not in the Graph?
        if (_mouseDownCoordinates == Coordinates.NaN)
        {
            return ResponseInfo.NoActionRequired;
        }
        // If the mouse is moved, adapt the rectangle
        if (userAction is IMouseAction mouseMoveAction and not IMouseButtonAction && _mouseIsDown)
        {
            _selectionRectangle.IsVisible = true;
            Coordinates mouseNowCoordinates = 
                plot.GetCoordinates(mouseMoveAction.Pixel.X, mouseMoveAction.Pixel.Y);

            _mouseSelectionRect = new CoordinateRect(_mouseDownCoordinates, mouseNowCoordinates);
            _selectionRectangle.CoordinateRect = _mouseSelectionRect;

            return ResponseInfo.Refresh;
        }
        
        if (userAction is IMouseButtonAction { IsPressed: false } mouseUpAction && mouseUpAction.Button == MouseButton)
        {
            _mouseDownCoordinates = Coordinates.NaN;
            
            if (_selectionRectangle.IsVisible)
            {
                _selectionRectangle.IsVisible = false;
                return ResponseInfo.Refresh;
            }
            _mouseIsDown = false;
        }

        return ResponseInfo.NoActionRequired;
    }
}