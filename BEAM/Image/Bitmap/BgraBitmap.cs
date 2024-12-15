using SkiaSharp;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BEAM.Image.Bitmap;

public sealed partial class BgraBitmap : SKBitmap, IBitmap<BGRA>
{
  public ref BGRA this[int x, int y]
  {
    get
    {
      Span<byte> bytes = GetPixelSpan().Slice(
        y * RowBytes +
        x * BytesPerPixel,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<BGRA>(bytes);
    }
  }

  public ref BGRA this[long i]
  {
    get
    {
      var x = (int)(i % Width);
      var y = (int)(i / Width);

      var bytes = GetPixelSpan().Slice(
        y * RowBytes +
        x * BytesPerPixel,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<BGRA>(bytes);
    }
  }

  public BgraBitmap(int width, int height) :
    base(width, height, SKColorType.Bgra8888, SKAlphaType.Premul)
  {
    Debug.Assert(Marshal.SizeOf<BGRA>() == BytesPerPixel);
  }


  public Span<byte> GetPixelSpan()
  {

    return new Span<byte>(Bytes, 0, Bytes.Length);
    /*return Bytes.AsSpan(
      BitmapHeaderSize,
      Bytes.Length - BitmapHeaderSize);*/
  }

  public void Read(string path)
  {
    throw new NotImplementedException("TODO");
  }

  public void Write(string path)
  {
    throw new NotImplementedException("TODO");
  }
}
