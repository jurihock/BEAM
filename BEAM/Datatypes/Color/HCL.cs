namespace BEAM.Datatypes.Color;


/// <summary>
/// Class representing Colors in any HCL format.
///
/// adapted from @jurihock
/// </summary>
public sealed class HueColorLut
{
    public double Saturation { get; init; }
    public double Value { get; init; }

    public HueColorLut(double saturation = 1, double value = 1)
    {
        Saturation = saturation;
        Value = value;
    }

    public BGR this[double hue] // [0..1]
    {
        get
        {
            var hsv = new HSV { H = hue, S = Saturation, V = Value };
            var bgr = hsv.ToBgr();

            return bgr;
        }
    }
}