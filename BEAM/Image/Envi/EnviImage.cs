using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace BEAM.Image.Envi;

/// <summary>
/// A base class used to cast its generic subclasses to a common type.
/// </summary>
public interface IEnviImage : IContiguousImage
{
}

/// <summary>
/// Decodes ENVI image data for the specified filename and value type T.
/// </summary>
public class EnviImage<T> : IContiguousImage<T>, IDisposable, IEnviImage
{
    private MemoryMappedFile? FileMapping { get; set; }
    private MemoryMappedViewAccessor? FileAccessor { get; set; }
    private Func<long, T> GetValue { get; init; }

    /// <summary>
    /// This image's dimensions, meaning its length, width and channel count.
    /// </summary>
    public ImageShape Shape { get; init; }
    /// <summary>
    /// The orientation pixels are being stored in memory with. 
    /// </summary>
    public ImageMemoryLayout Layout { get; init; }

    /// <summary>
    /// Gets a specific pixel value's channel by coordinates.
    /// </summary>
    /// <param name="x">The pixel's width coordinate.</param>
    /// <param name="y">The pixel's height coordinate.</param>
    /// <param name="z">The pixel's channel.</param>
    public T this[long x, long y, int z] => GetValue(Layout.Flatten(x, y, z));
    
    /// <summary>
    /// Gets a specific pixel value's channel by the pixels position in a flattened one dimensional image, meaning by its position in the raw filestream.
    /// </summary>
    /// <param name="i">The pixel's position in a one dimensional array according to the <see cref="ImageMemoryLayout"/>.</param>
    public T this[long i] => GetValue(i);

    /// <summary>
    /// Returns a pixel channel's value, converted to a double.
    /// </summary>
    /// <param name="i">The pixel's position in a one dimensional array according to the <see cref="ImageMemoryLayout"/>.</param>
    /// <returns>The specified pixel-channel value as a double.</returns>
    public double GetAsDouble(long i)
    {
        return (double) Convert.ChangeType(this[i], typeof(double))!;
    }

    /// <summary>
    /// Returns a pixel channel's value, converted to a double.
    /// </summary>
    /// <param name="x">The pixel's width coordinate.</param>
    /// <param name="y">The pixel's height coordinate.</param>
    /// <param name="z">The pixel's channel.</param>
    /// <returns>The specified pixel-channel value as a double.</returns>
    public double GetAsDouble(long x,long y, int z)
    {
        return (double) Convert.ChangeType(this[x, y, z], typeof(double))!;
    }

    /// <summary>
    /// Creates a new instance given the filepath to an envis header and raw file, whose names must match except for their file endings.
    /// The default file endings of .hdr for header files and .raw for data files must be used.
    /// </summary>
    /// <param name="filepath">The path to both envi files including their name. Therefor their names must match.</param>
    public EnviImage(string filepath) : this(filepath, (hdr: ".hdr", raw: ".raw"))
    {
    }

    /// <summary>
    /// Creates a new instance given the filepath to an envis header and raw file, whose names must match except for their file endings.
    /// The extensions for the different types (header and data) can be customized.
    /// </summary>
    /// <param name="filepath">The path to both envi files including their name. Therefor their names must match.</param>
    /// <param name="extensions">A tuple whose first entry is the file extension for the header file and its second is the data files extension.</param>
    /// <exception cref="FileNotFoundException">If either file was found at the specified filepath.</exception>
    /// <exception cref="NotSupportedException">If no envi file, or an ENVI file with a byte order other than the host's was found.</exception>
    public EnviImage(string filepath, (string hdr, string raw) extensions)
    {
        var filepaths = (
            hdr: Path.ChangeExtension(filepath, extensions.hdr),
            raw: Path.ChangeExtension(filepath, extensions.raw));

        if (!File.Exists(filepaths.hdr))
        {
            throw new FileNotFoundException(
                $"Missing ENVI header file \"{filepaths.hdr}\"!");
        }

        if (!File.Exists(filepaths.raw))
        {
            throw new FileNotFoundException(
                $"Missing ENVI raw file \"{filepaths.raw}\"!");
        }

        var header = new EnviHeader(filepaths.hdr);

        var filetype = header.Get<string>("file type", "ENVI");
        var offset = header.Get<int>("header offset", 0);

        var byteorder = header.Get<EnviByteOrder>("byte order", EnviExtensions.GetNativeByteOrder());
        var datatype = header.Get<EnviDataType>("data type");
        var interleave = header.Get<EnviInterleave>("interleave");

        var width = header.Get<int>("samples");
        var height = header.Get<int>("lines");
        var channels = header.Get<int>("bands");

        if (!filetype.Contains("ENVI", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException(
                $"Invalid ENVI file type \"{filetype}\"!");
        }

        if (!byteorder.IsNative())
        {
            throw new NotSupportedException(
                $"Incompatible ENVI byte order \"{byteorder}\"!");
        }

        if (datatype.IsComplex())
        {
            throw new NotSupportedException(
                $"Unsupported ENVI data type \"{datatype}\"!");
        }

        var size = 1L * width * height * channels * datatype.SizeOf();

        FileMapping = MemoryMappedFile.CreateFromFile(
            filepaths.raw,
            FileMode.OpenOrCreate,
            null,
            size + offset,
            MemoryMappedFileAccess.Read);

        FileAccessor = FileMapping.CreateViewAccessor(
            offset,
            size,
            MemoryMappedFileAccess.Read);

        GetValue = FileAccessor.CreateValueGetter<T>(datatype);

        Shape = new ImageShape(
            width,
            height,
            channels);

        Layout = interleave switch
        {
            EnviInterleave.BIP => new YxzImageMemoryLayout(Shape),
            EnviInterleave.BIL => new YzxImageMemoryLayout(Shape),
            EnviInterleave.BSQ => new ZyxImageMemoryLayout(Shape),
            _ => throw new NotSupportedException(
                $"Unsupported ENVI interleave \"{interleave}\"!")
        };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (FileAccessor != null)
        {
            FileAccessor.Dispose();
            FileAccessor = null;
        }
    }
}

/// <summary>
/// This class contains the utility to open new ENVI files. 
/// </summary>
public static class EnviImage
{
    /// <summary>
    /// This method opens a new file as an envi file given the supplied path. Its corresponding header file must be located at the same location with the same name.
    /// </summary>
    /// <param name="filepath">The path to the ENVI file (header and data).</param>
    /// <returns> A generic <see cref="EnviImage{T}"/> cast to its superclass <see cref="IEnviImage"/> which represents the files found under the supplied path.</returns>
    /// <exception cref="FileNotFoundException">If either the header or the raw file were not found in under the supplied path.</exception>
    public static IEnviImage OpenImage(string filepath)
    {
        var filepaths = (
            hdr: Path.ChangeExtension(filepath, ".hdr"),
            raw: Path.ChangeExtension(filepath, ".raw"));

        if (!File.Exists(filepaths.hdr))
        {
            throw new FileNotFoundException(
                $"Missing ENVI header file \"{filepaths.hdr}\"!");
        }

        if (!File.Exists(filepaths.raw))
        {
            throw new FileNotFoundException(
                $"Missing ENVI raw file \"{filepaths.raw}\"!");
        }

        var header = new EnviHeader(filepaths.hdr);
        var datatype = header.Get<EnviDataType>("data type");

        var type = datatype.TypeOf();
        var ctor = typeof(EnviImage<>).MakeGenericType([type]).GetConstructor([typeof(string)])!;
        return (IEnviImage)ctor.Invoke([filepath]);
    }
}