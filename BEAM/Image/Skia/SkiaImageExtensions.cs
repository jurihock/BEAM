using SkiaSharp;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BEAM.Image.Envi;

namespace BEAM.Image.Skia;
/// <summary>
///  This class provides utility Methods for other Skia-Image-associated classes like <see cref="SKColorType"/> and <see cref="SKBitmap"/>.
/// </summary>
public static class SkiaImageExtensions
{
    /// <summary>
    /// Returns the type of a channel's data for a given color type. Supported types all have values in the range of a byte.
    /// </summary>
    /// <param name="type">The color type whose data type per channel is meant to be retrieved.</param>
    /// <returns>The size of the corresponding primitive c# type to the given color type.</returns>
    /// <exception cref="NotSupportedException">If the supplied color type is not supported.</exception>
    public static Type TypeOf(this SKColorType type)
    {
        return type switch
        {
            SKColorType.Gray8 => typeof(byte),
            SKColorType.Bgra8888 => typeof(byte),
            SKColorType.Rgba8888 => typeof(byte),
            SKColorType.Rgb888x => typeof(byte),
            _ => throw new NotSupportedException(
                $"Unsupported Skia data type \"{type}\"!")
        };
    }

    /// <summary>
    /// Returns the size of a given color type's channel data type.
    /// </summary>
    /// <param name="type">The type color whose channel data type's size is meant to be returned.</param>
    /// <returns>Its channel data type's size.</returns>
    public static int SizeOf(this SKColorType type)
    {
        return Marshal.SizeOf(type.TypeOf());
    }

    /// <summary>
    /// Reads the value of a data type from the bitmap using its raw bytes.
    /// </summary>
    /// <param name="bmp">The bitmap from which the data is being read.</param>
    /// <param name="offset">The byte offset from the bitmaps beginning. Parsing will start here.</param>
    /// <typeparam name="T">The type which is meant to be read from the raw data.</typeparam>
    /// <returns>The data of type T read from the bytes of the bitmap starting at the offset.</returns>
    public static unsafe T UnsafeRead<T>(this SKBitmap bmp, long offset)
    {
        return Unsafe.Read<T>((byte*)bmp.GetPixels() + offset);
    }

    /// <summary>
    /// Creates a function which takes an offset to a file as an argument and returns an object of the T's type.
    /// The offset determines how many readable objects are skipped. This means that an offset of 2 means the third readable object is being returned.
    /// Format: (long index) => (T)(UnsafeRead(bmp, index * bmp.ColorType.SizeOf()));
    /// </summary>
    /// <param name="bmp">The bitmap used to get data bytes from.</param>
    /// <typeparam name="T">A specific c# type. The data in the accessor's file will be read as if it were representing this type.</typeparam>
    /// <returns>The offset'th readable element from the file represented by the accessor of the type which is represented by <see cref="EnviDataType"/>.</returns>
    public static Func<long, T> CreateValueGetter<T>(this SKBitmap bmp)
    {
        var bitmap = Expression.Constant(bmp);
        var method = typeof(SkiaImageExtensions).GetMethod(nameof(UnsafeRead))!
            .MakeGenericMethod(bmp.ColorType.TypeOf());

        var index = Expression.Parameter(typeof(long));
        var bytes = Expression.Constant((long)bmp.ColorType.SizeOf());
        var offset = Expression.Multiply(index, bytes);

        var input = Expression.Call(method, bitmap, offset);
        var output = Expression.Convert(input, typeof(T));

        return Expression.Lambda<Func<long, T>>(output, index).Compile();
    }

    /// <summary>
    /// Creates a function which can be used to rearrange the indices for accessing the bitmap element's channels so that the access fits a bgr style bitmap.
    /// </summary>
    /// <param name="bmp">The bitmaps for which the index switch function is meant to be created.</param>
    /// <returns> A function which maps channels correctly. Meaning channel numbers are being switched if the supplied bitmap is not already in the bgr format.</returns>
    public static Func<int, int> CreateColorChannelDecoder(this SKBitmap bmp)
    {
        var rgb = bmp.ColorType.ToString().StartsWith(
            "rgb", StringComparison.OrdinalIgnoreCase);

        if (rgb) // swap r and b
        {
            return index => index switch
            {
                0 => 2,
                2 => 0,
                _ => index
            };
        }

        // already bgr
        return index => index;
    }
}