using Avalonia.Controls;

namespace BEAM.Views.Utility;

/// <summary>
/// A savable user control. This is used to ensure that changes made by the user will be transfered.
/// </summary>
public abstract class SaveUserControl : UserControl
{
    /// <summary>
    /// Saves the views data.
    /// </summary>
    public abstract void Save();
}