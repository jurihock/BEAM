using System;

namespace BEAM.Image.Bitmap;

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
