using SkiaSharp;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BEAM.Datatypes.Color;

namespace BEAM.Image.Bitmap;

/// <summary>
/// A wrapper around <see cref="SKBitmap"/>, with the ability to modify the data of the bitmap by accessing it's memory layout.
/// Used to access pixel data as <see cref="BGRA"/> objects
/// </summary>
public sealed class BgraBitmap : SKBitmap, IBitmap<BGRA>
{
    public ref BGRA this[int x, int y]
    {
        get
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }
            var bytes = GetPixelSpan().Slice(
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


    /// <summary>
    /// Returns a writable Span of the data's (byte array)  as bytes. This is a workaround in comparison
    /// to the less permissive SKBitmap superclass.
    /// </summary>
    /// <returns>A byte span to the memory region of the pixel data.</returns>
    public new unsafe Span<byte> GetPixelSpan()
    {
        return new Span<byte>((void*)GetPixels(out var length), (int)length);
    }

    public void Read(string path)
    {
        throw new NotImplementedException();
    }

    public void Write(string path)
    {
        throw new NotImplementedException();
    }
}
