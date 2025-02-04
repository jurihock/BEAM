using System.Dynamic;
using ScottPlot;

namespace BEAM.Analysis;

public class AnalysisResult<T>
{
    public AnalysisResult(T data, AnalysisDisplayType displayType)
    {
        Data = data;
        DisplayType = displayType;
    }
    public AnalysisDisplayType DisplayType { get; }
    public T Data  { get; }
    
}