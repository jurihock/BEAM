using System;

namespace BEAM.Image.Bitmap;

/// <summary>
/// Interface for a custom bitmap, used to access the underlying pixel values as <see cref="T"/> objects.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IBitmap<T>
{
  int Width { get; }
  int Height { get; }

  byte[] Bytes { get; }

  ref T this[int x, int y] { get; }
  ref T this[long i] { get; }

  Span<byte> GetPixelSpan();

  void Read(string path);
  void Write(string path);
}
