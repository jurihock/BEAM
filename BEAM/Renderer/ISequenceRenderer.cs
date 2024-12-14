namespace BEAM.Renderer;

public interface ISequenceRenderer
{
    /// <summary>
    /// Returns an array of size 3
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    byte[] RenderPixel(double[] data);
}