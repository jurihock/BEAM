

namespace BEAM.ViewModels.Utility;

/// <summary>
/// An abstract class for a view model which can be notified about changes of its bounds.
/// It can therefore change its size accordingly.
/// </summary>
public abstract class SizeAdjustableViewModelBase : ViewModelBase
{
    /// <summary>
    /// Event delegate used for invoking the size changed event.
    /// </summary>
    public delegate void SizeChangedEventHandler(object sender, SizeChangedEventArgs e);
    
    /// <summary>
    /// Event used to notify of a change in size.
    /// </summary>
    protected event SizeChangedEventHandler SizeChanged = delegate { };

    /// <summary>
    /// Used to subscribe to the size changed event. Caller will be notified
    /// of the event invocation through the function provided.
    /// </summary>
    /// <param name="handleFunc">The function to call back on.</param>
    public void NotifyOnSizeChanged(SizeChangedEventHandler handleFunc)
    {
        SizeChanged += handleFunc;
    }
    
    /// <summary>
    /// Used to unsubscribe from the size changed event. 
    /// </summary>
    /// <param name="handleFunc">The function which was formerly used toc all back on and which will now be removed.</param>
    public void RemoveNotifyOnSizeChanged(SizeChangedEventHandler handleFunc)
    {
        SizeChanged -= handleFunc;
    }
    
    /// <summary>
    /// Used to invoke the event, informing all listeners to a size change.
    /// </summary>
    /// <param name="sender">The origin of the invocation, meaning the caller.</param>
    /// <param name="e">The <see cref="SizeChangedEventArgs"/> which contain information about the new size.</param>
    public void NotifySizeChanged(object sender, SizeChangedEventArgs e)
    {
        SizeChanged.Invoke(sender, e);
    }
}