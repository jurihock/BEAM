using ScottPlot;

namespace BEAM.Analysis;

public class BarChartResult : AnalysisResult<double[]>
{

    public BarChartResult(AnalysisDisplayType displayType, double[] data) : base(data, displayType)
    {
    }
}