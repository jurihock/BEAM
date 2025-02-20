

namespace BEAM.ViewModels.Utility;

public abstract class SizeAdjustableViewModelBase : ViewModelBase
{
    public delegate void SizeChangedEventHandler(object sender, SizeChangedEventArgs e);
    protected event SizeChangedEventHandler SizeChanged = delegate { };

    public void NotifyOnSizeChanged(SizeChangedEventHandler handleFunc)
    {
        SizeChanged += handleFunc;
    }
    public void RemoveNotifyOnSizeChanged(SizeChangedEventHandler handleFunc)
    {
        SizeChanged -= handleFunc;
    }
    
    public void NotifySizeChanged(object sender, SizeChangedEventArgs e)
    {
        SizeChanged.Invoke(sender, e);
    }
}