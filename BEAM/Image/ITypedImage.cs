// (c) Paul Stier, 2025

namespace BEAM.Image;

/// <summary>
/// Interface for an image class that may contain its data in a variable format.
/// </summary>
/// <typeparam name="T">The type of data stored in the image</typeparam>
public interface ITypedImage<T> : IImage
{
    T this[long x, long y, int channel] { get; }
}