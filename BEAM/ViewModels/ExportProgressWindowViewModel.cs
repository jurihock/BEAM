using System;
using System.Threading;

namespace BEAM.ViewModels;

/// <summary>
/// ViewModel for the Export progress window popup.
/// </summary>
/// <param name="ctxSource">A CancellationToken which is to be used for by the process to answer cancellation requests.</param>
public class ExportProgressWindowViewModel(CancellationTokenSource ctxSource) : ViewModelBase
{
    private byte _actionProgress = 0;
    /// <summary>
    /// The progress of the export in the range [0,100]
    /// </summary>
    public byte ActionProgress
    {
        get => _actionProgress;
        set
        {
            if (_actionProgress == value) return;
            _actionProgress = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the cancellation token for the export
    /// operation which is linked to this vm and used by the popup for cancel requests.
    /// </summary>
    /// <returns>The <see cref="CancellationToken"/> used to inform about an export process cancellation.</returns>
    public CancellationToken GetCancellationToken() => ctxSource.Token;
    

    /// <summary>
    /// Event used to inform that the export has ended and all views linked to this vm should close.
    /// </summary>
    public EventHandler<CloseEventArgs> CloseEvent { get; set; } = delegate { };

    /// <summary>
    /// Aborts the generation process by cancelling the token.
    /// </summary>
    public void AbortGeneration()
    {
        ctxSource.Cancel();
        Close();
    }

    /// <summary>
    /// Invokes the CloseEvent.
    /// </summary>
    public void Close() => CloseEvent(this, new CloseEventArgs());
}