// (c) Paul Stier, 2025

namespace BEAM.Image;

public interface ITypedImage<T> : IImage
{
    T this[long x, long y, int channel] { get; }
}