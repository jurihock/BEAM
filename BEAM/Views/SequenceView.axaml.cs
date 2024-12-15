using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BEAM.Image.Bitmap;
using BEAM.IMage.Displayer.Scottplot;
using ScottPlot.Avalonia;

namespace BEAM.Views;

public partial class SequenceView : UserControl
{
    public SequenceView()
    {
        InitializeComponent();
        ScottPlotTest();
    }

    private void ScottPlotTest()
    {
        var avaPlot1 = this.Find<AvaPlot>("AvaPlot1");

        if (avaPlot1 == null)
        {
            return;
        }

        // TODO: CustomMouseActions
        // https://github.com/ScottPlot/ScottPlot/blob/main/src/ScottPlot5/ScottPlot5%20Demos/ScottPlot5%20WinForms%20Demo/Demos/CustomMouseActions.cs

        //avaPlot1.Interaction.IsEnabled = true;
        //avaPlot1.UserInputProcessor.IsEnabled = true;
        //avaPlot1.UserInputProcessor.UserActionResponses.Clear();

        //var panButton = ScottPlot.Interactivity.StandardMouseButtons.Middle;
        //var panResponse = new ScottPlot.Interactivity.UserActionResponses.MouseDragPan(panButton);

        var bitmap = new BgraBitmap(1 * 1000, 1 * 1000);
        bitmap.Fill(b: 100, g: 0, r: 100, a: 255);

        var plottable = new BitmapPlottable(bitmap);
        avaPlot1.Plot.Add.Plottable(plottable);
        avaPlot1.Plot.Axes.InvertY();

        avaPlot1.Refresh();
    }
    
}