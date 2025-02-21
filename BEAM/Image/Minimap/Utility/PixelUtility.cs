using BEAM.Datatypes.Color;

namespace BEAM.Image.Minimap.Utility;

public static class PixelUtility
{
    public static bool EntrywiseAllGreaterEqual(this BGR baseline, BGR other)
    {

        return (baseline.R >= other.R) && (baseline.G >= other.G) && (baseline.B >= other.B);
    }
    
    public static bool EntrywiseAnyGreater(this BGR baseLine, BGR other)
    {
        return (baseLine.R > other.R) || (baseLine.G > other.G) || (baseLine.B > other.B);
    }
}