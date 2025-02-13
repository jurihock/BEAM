namespace BEAM.Datatypes.Color;


/// <summary>
/// Class representing Colors in any HCL format, additionally with an alpha value for transparency.
///
/// adapted from @jurihock
/// </summary>
public sealed class HueColorLutAlpha
{
    public double Saturation { get; init; }
    public double Value { get; init; }

    public HueColorLutAlpha(double saturation = 1, double value = 1)
    {
        Saturation = saturation;
        Value = value;
    }

    public BGRA this[double hue, double transparency] // [0..1]
    {
        get
        {
            var hsva = new HSVA { H = hue, S = Saturation, V = Value, A = transparency };
            var bgra = hsva.ToBGRA();

            return bgra;
        }
    }
}