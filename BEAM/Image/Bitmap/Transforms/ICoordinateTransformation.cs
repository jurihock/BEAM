namespace BEAM.Image.Bitmap.Transforms;

public interface ICoordinateTransformation<T>
{
  T Forward(T value);
  T Backward(T value);
}