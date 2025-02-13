using System;

namespace BEAM.Datatypes.Color;

/// <summary>
/// Allows conversion between BGRA and HSV color formats.
///
/// adapted from @jurihock
/// </summary>
public static class ColorExtensions
{
    private const double TOLERANCE = 0.0001;
    
    public static HSVA ToHSVA(this BGRA bgra)
    {
        // adapted from https://gist.github.com/mjackson/5311256
        // see also     https://www.cs.rit.edu/~ncs/color/t_convert.html

        double r = bgra.R;
        double g = bgra.G;
        double b = bgra.B;
        double a = bgra.A;

        r /= 255;
        g /= 255;
        b /= 255;
        a /= 255;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));

        var h = max;
        var s = max;
        var v = max;

        var d = max - min;

        s = max == 0 ? 0 : d / max;

        if (Math.Abs(max - min) < TOLERANCE)
        {
            h = 0;
        }
        else
        {
            if (Math.Abs(r - max) < TOLERANCE)
            {
                h = (g - b) / d + (g < b ? 6 : 0);
            }
            else if (Math.Abs(g - max) < TOLERANCE)
            {
                h = (b - r) / d + 2;
            }
            else if (Math.Abs(b - max) < TOLERANCE)
            {
                h = (r - g) / d + 4;
            }

            h /= 6;
        }

        h = Math.Clamp(h, 0, 1);
        s = Math.Clamp(s, 0, 1);
        v = Math.Clamp(v, 0, 1);

        return new HSVA { H = h, S = s, V = v , A = a};
    }

    public static BGRA ToBGRA(this HSVA hsva)
    {
        // adapted from https://gist.github.com/mjackson/5311256
        // see also     https://www.cs.rit.edu/~ncs/color/t_convert.html

        double h = hsva.H;
        double s = hsva.S;
        double v = hsva.V;
        double a = hsva.A;

        double r = 0;
        double g = 0;
        double b = 0;

        var i = Math.Floor(h * 6);
        var f = h * 6 - i;
        var p = v * (1 - s);
        var q = v * (1 - f * s);
        var t = v * (1 - (1 - f) * s);

        switch (i % 6)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
        }

        r *= 255;
        g *= 255;
        b *= 255;
        a *= 255;

        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);
        a = Math.Clamp(b, 0, 255);

        return new BGRA { R = (byte)r, G = (byte)g, B = (byte)b , A = (byte)a};
    }
}