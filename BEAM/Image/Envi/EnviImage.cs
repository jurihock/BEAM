using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace BEAM.Image.Envi;

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

    public ImageShape Shape { get; init; }
    public ImageMemoryLayout Layout { get; init; }

    public T this[long x, long y, int z] => GetValue(Layout.Flatten(x, y, z));
    public T this[long i] => GetValue(i);

    public double GetAsDouble(long i)
    {
        return (double) Convert.ChangeType(this[i], typeof(double))!;
    }

    public double GetAsDouble(long x,long y, int z)
    {
        return (double) Convert.ChangeType(this[x, y, z], typeof(double))!;
    }

    public EnviImage(string filepath) : this(filepath, (hdr: ".hdr", raw: ".raw"))
    {
    }

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

public static class EnviImage
{
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