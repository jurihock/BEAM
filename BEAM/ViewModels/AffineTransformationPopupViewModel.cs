// (c) Paul Stier, 2025

using BEAM.Renderer;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BEAM.ViewModels;

/// <summary>
/// View model for controlling the affine transformation setting popup.
/// </summary>
/// <param name="model"></param>
public partial class AffineTransformationPopupViewModel(SequenceViewModel model) : ViewModelBase
{
    /// <summary>
    /// The x position offset to draw the sequence at.
    /// </summary>
    [ObservableProperty]
    public partial decimal OffsetX { get; set; } = (decimal)model.Sequence.DrawOffsetX;

    /// <summary>
    /// The y position offset to draw the sequence at.
    /// </summary>
    [ObservableProperty]
    public partial decimal OffsetY { get; set; } = (decimal)model.Sequence.DrawOffsetY;

    /// <summary>
    /// The x scale to draw the sequence at.
    /// </summary>
    [ObservableProperty]
    public partial decimal ScaleX { get; set; } = (decimal)model.Sequence.ScaleX;

    /// <summary>
    /// The x scale to draw the sequence at.
    /// </summary>
    [ObservableProperty]
    public partial decimal ScaleY { get; set; } = (decimal)model.Sequence.ScaleY;

    /// <summary>
    /// Saves the entered settings and redraws the sequence.
    /// </summary>
    /// <returns>Whether the settings were applied successfully</returns>
    public bool Save()
    {
        model.Sequence.DrawOffsetX = (double)OffsetX;
        model.Sequence.DrawOffsetY = (double)OffsetY;
        model.Sequence.ScaleX = (double)ScaleX;
        model.Sequence.ScaleY = (double)ScaleY;
        model.RenderersUpdated.Invoke(this, new RenderersUpdatedEventArgs());
        return true;
    }
}