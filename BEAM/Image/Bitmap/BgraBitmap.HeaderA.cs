using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BEAM.Image.Bitmap;

public sealed partial class BgraBitmap
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  private readonly struct HeaderA
  {
    public ushort FileType { get; init; }
    public uint FileSize { get; init; }
    public uint Reserved { get; init; }
    public uint DataOffset { get; init; }
    public uint InfoSize { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public ushort Planes { get; init; }
    public ushort BitsPerPixel { get; init; }
    public uint Compression { get; init; }
    public uint ImageSize { get; init; }
    public int PixelsPerMeterX { get; init; }
    public int PixelsPerMeterY { get; init; }
    public uint ColorsUsed { get; init; }
    public uint ColorsImportant { get; init; }

    public int BitsPerLine => (Width * BitsPerPixel + 31) / 32;
    public int BytesPerLine => BitsPerLine * 4;

    public HeaderA(int width, int height)
    {
      Debug.Assert(Marshal.SizeOf<HeaderA>() == BitmapHeaderSize);

      Width = width;
      Height = height;

      BitsPerPixel = BitmapBitsPerPixel;
      Planes = 1;

      FileType = 0x4D42;
      FileSize = BitmapHeaderSize + (uint)(height * BytesPerLine);
      ImageSize = (uint)(height * BytesPerLine);
      InfoSize = BitmapInfoHeaderSize;
      DataOffset = BitmapHeaderSize;
    }
  }
}
