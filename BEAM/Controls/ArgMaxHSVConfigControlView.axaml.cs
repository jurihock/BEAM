using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
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
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void Save()
    {
        ((DataContext as ArgMaxHSVConfigControlViewModel)!).Save();
    }
}