using System;
using System.Runtime.InteropServices;

namespace BEAM.Image.Bitmap;

public static class BitmapExtensions
{
  public static IBitmap<BGR> Fill(this IBitmap<BGR> bitmap, byte b, byte g, byte r)
  {
    return bitmap.Fill(new BGR() { B = b, G = g, R = r });
  }

  public static IBitmap<BGR> Fill(this IBitmap<BGR> bitmap, BGR pixel)
  {
    var bytes = bitmap.GetPixelSpan();

    var pixels = MemoryMarshal.CreateSpan(
      ref MemoryMarshal.AsRef<BGR>(bytes),
      bytes.Length / Marshal.SizeOf<BGR>());

    pixels.Fill(pixel);

    return bitmap;
  }

  public static IBitmap<BGRA> Fill(this IBitmap<BGRA> bitmap, byte b, byte g, byte r, byte a)
  {
    return bitmap.Fill(new BGRA() { B = b, G = g, R = r, A = a });
  }

  public static IBitmap<BGRA> Fill(this IBitmap<BGRA> bitmap, BGRA pixel)
  {
    var bytes = bitmap.GetPixelSpan();

    /*var pixels = MemoryMarshal.CreateSpan(
      ref MemoryMarshal.AsRef<BGRA>(bytes),
      bytes.Length / Marshal.SizeOf<BGRA>());*/
    var pixels = MemoryMarshal.Cast<byte, BGRA>(bytes);
    if (bytes.Length % Marshal.SizeOf<BGRA>() != 0)
    {
      //throw new InvalidOperationException("Pixel span size is not a multiple of BGRA size.");
      Console.WriteLine(bytes.Length % Marshal.SizeOf<BGRA>());
      Console.WriteLine(bytes.Length);
      Console.WriteLine(Marshal.SizeOf<BGRA>());
      Console.WriteLine("Length missmatch");
    }

    pixels.Fill(pixel);

    return bitmap;
  }
}
