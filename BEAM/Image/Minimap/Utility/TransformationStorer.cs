using BEAM.ImageSequence;

namespace BEAM.Image.Minimap.Utility;

public struct TransformationStorer
{
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public double ScaleX { get; set; }
    public double ScaleY { get; set; }

    public void Update(ISequence sequence)
    {
        if (!sequence.GetType().IsAssignableTo(typeof(TransformedSequence)))
        {
            OffsetX = 0;
            OffsetY = 0;
            ScaleX = 1;
            ScaleY = 1;
        }
        else
        {
            var transformedSequence = (TransformedSequence)sequence;
            OffsetX = transformedSequence.DrawOffsetX;
            OffsetY = transformedSequence.DrawOffsetY;
            ScaleX = transformedSequence.ScaleX;
            ScaleY = transformedSequence.ScaleY;
        }
    }
}