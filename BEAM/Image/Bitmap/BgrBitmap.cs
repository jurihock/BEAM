using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using BEAM.Datatypes.Color;

namespace BEAM.Image.Bitmap;

public sealed partial class BgrBitmap : IBitmap<BGR>
{
  private const int BytesPerPixel = BitmapBytesPerPixel;
  private int BytesPerLine;

  public int Width { get; private set; }
  public int Height { get; private set; }

  public byte[] Bytes { get; private set; } = [];

  public ref BGR this[int x, int y]
  {
    get
    {
      var bytes = Bytes.AsSpan(
        y * BytesPerLine +
        x * BytesPerPixel +
        BitmapHeaderSize,
        BytesPerPixel);

      return ref MemoryMarshal.AsRef<BGR>(bytes);
    }
  }

  public ref BGR this[long i]
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

      return ref MemoryMarshal.AsRef<BGR>(bytes);
    }
  }

  public BgrBitmap(int width, int height)
  {
    Debug.Assert(Marshal.SizeOf<BGR>() == BytesPerPixel);

    var header = new Header(width, height);
    var bytes = new byte[header.FileSize];

    MemoryMarshal.Write(bytes, header);

    BytesPerLine = header.BytesPerLine;
    Width = header.Width;
    Height = header.Height;
    Bytes = bytes;
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
    var header = MemoryMarshal.Read<Header>(bytes);

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
