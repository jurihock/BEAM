using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace BEAM.Image.Bitmap;

public sealed partial class BgraBitmap : SKBitmap, IBitmap<BGRA>
{
  private const int BytesPerPixel = BitmapBytesPerPixel;
  private int BytesPerLine;

  public new byte[] Bytes { get; private set; }
  
  public new int Width { get; private set; }
  public new int Height { get; private set; }
  
  public ref BGRA this[int x, int y]
  {
    get
    {
      var bytes = Bytes.AsSpan(
        y * BytesPerLine +
        x * BytesPerPixel +
        BitmapHeaderSize,
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

      var bytes = Bytes.AsSpan(
        y * BytesPerLine +
        x * BytesPerPixel +
        BitmapHeaderSize,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<BGRA>(bytes);
    }
  }

  public BgraBitmap(int width, int height) :
    base(width, height, SKColorType.Bgra8888, SKAlphaType.Premul)
  {
    var header = new HeaderA(width, height);
    var bytes = new byte[header.FileSize];

    MemoryMarshal.Write(bytes, header);

    BytesPerLine = header.BytesPerLine;
    Width = header.Width;
    Height = header.Height;
    Bytes = bytes;
    Debug.Assert(Marshal.SizeOf<BGRA>() == BytesPerPixel);
    
  }


  public Span<byte> GetPixelSpan()
  {

    return Bytes.AsSpan(
      BitmapHeaderSize,
      Bytes.Length - BitmapHeaderSize);
  }

  public void Read(string path)
  {
    var bytes = File.ReadAllBytes(path);
    var header = MemoryMarshal.Read<BgraBitmap.HeaderA>(bytes);

    BytesPerLine = header.BytesPerLine;
    Width = header.Width;
    Height = header.Height;
    Bytes = bytes;
  }

  public void Write(string path)
  {
    File.WriteAllBytes(path, Bytes);
  }
}
