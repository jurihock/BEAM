using Avalonia.Controls;
using BEAM.Renderer;
using BEAM.ViewModels;

namespace BEAM.Controls;

public partial class ArgMaxHSVConfigControlView : UserControl, ISaveControl
{
    public ArgMaxHSVConfigControlView(ArgMaxRendererColorHSV renderer, SequenceViewModel model)
    {
        DataContext = new ArgMaxHSVConfigControlViewModel(renderer, model);
        InitializeComponent();
    }

    public void Save()
    {
        ((DataContext as ArgMaxHSVConfigControlViewModel)!).Save();
    }
}