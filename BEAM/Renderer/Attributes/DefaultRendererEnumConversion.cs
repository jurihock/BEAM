using System;

namespace BEAM.Renderer.Attributes;

/// <summary>
/// This class provides utility methods to convert <see cref="RenderTypes"/> to <see cref="SequenceRenderer"/>,
/// hence mapping the enum representation to an actual renderer.
/// </summary>
public static class DefaultRendererEnumConversion
{
    /// <summary>
    /// Gets a default SequenceRenderer based on this RendererEnum supplied.
    /// </summary>
    /// <param name="renderer">The enum value whose corresponding actual renderer shall be retrieved.</param>
    /// <param name="min">The minimum pixel value for the renderer.</param>
    /// <param name="max">The maximum pixel value for the renderer.</param>
    /// <returns>The actual <see cref="SequenceRenderer"/> correspnding to this enum entry.</returns>
    /// <exception cref="ArgumentException">If there is no corresponding renderer defined for this enum entry.</exception>
    public static SequenceRenderer Sequence(this RenderTypes renderer, int min, int max)
    {
        return renderer switch
        {       
            RenderTypes.ChannelMapRenderer => new ChannelMapRenderer(min, max, 2, 1, 0),
            RenderTypes.HeatMapRendererRb => new HeatMapRendererRB(min, max, 0, 0.1, 0.9),
            RenderTypes.ArgMaxRendererGrey => new ArgMaxRendererGrey(min, max),
            RenderTypes.ArgMaxRendererColorHsva => new ArgMaxRendererColorHSV(min, max),
            _ => throw new ArgumentException(null, nameof(renderer))
        };
    }
}