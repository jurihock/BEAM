namespace BEAM.Renderer;

public interface ISequenceRenderer
{
    /// <summary>
    /// Returns an array of size 3. These three values
    /// equal the R, G and B value of the pixel
    /// </summary>
    /// <param name="data"></param>
    /// <param name="displayParameters"><\param>
    /// <returns></returns>
    byte[] RenderPixel(double[] data, double[] displayParameters);
}