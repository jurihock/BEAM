using System.Linq;

namespace BEAM.Image.Bitmap.Transforms;

public sealed class LinearCoordinateTransformation : ICoordinateTransformation<double>
{
  public double Slope { get; private set; }
  public double Intercept { get; private set; }

  public LinearCoordinateTransformation(double slope = 1, double intercept = 0)
  {
    Slope = slope;
    Intercept = intercept;
  }

  public LinearCoordinateTransformation(double[] values)
  {
    var min = values.Min();
    var max = values.Max();
    var num = values.Length;

    Slope = (max - min) / (num - 1);
    Intercept = min;
  }

  public LinearCoordinateTransformation(float[] values)
  {
    var min = values.Min();
    var max = values.Max();
    var num = values.Length;

    Slope = (max - min) / (num - 1);
    Intercept = min;
  }

  public double Forward(double value)
  {
    return value * Slope + Intercept;
  }

  public double Backward(double value)
  {
    return (value - Intercept) / Slope;
  }
}