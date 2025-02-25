using BEAM.Image.Envi;
using PureHDF;
using PureHDF.VOL.Native;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;

namespace BEAM.Image.Hdf;

/// <summary>
/// Decodes ENVI image data for the specified filename and value type T.
/// </summary>
public class HdfImage<T> : ITypedImage<T>, IMemoryImage
{
    private string FilePath { get; set; }

    private MemoryMappedFile? FileMapping { get; set; }
    private MemoryMappedViewAccessor? FileAccessor { get; set; }

    private NativeFile? Datafile { get; set; }
    private IH5Dataset? Dataset { get; set; }

    private Func<long, long, long, T> GetValue { get; init; }
    private Func<long, long, int, double> GetDoubleValue { get; init; }
    private Action<long, long, int[], double[]> GetDoubleValues { get; init; }

    /// <summary>
    /// This image's dimensions, meaning its length, width and channel count.
    /// </summary>
    public ImageShape Shape { get; init; }

    /// <summary>
    /// The orientation pixels are being stored in memory with. 
    /// </summary>
    public ImageMemoryLayout Layout { get; init; }

    /// <summary>
    /// Creates a new instance given the filepath to an envis header and raw file, whose names must match except for their file endings.
    /// The default file endings of .hdr for header files and .raw for data files must be used.
    /// </summary>
    /// <param name="filepath">The path to both envi files including their name. Therefor their names must match.</param>
    public HdfImage(string filepath) : this(filepath, "Data/Image")
    {
    }

    /// <summary>
    /// Creates a new instance given the filepath to an envis header and raw file, whose names must match except for their file endings.
    /// The extensions for the different types (header and data) can be customized.
    /// </summary>
    /// <param name="filepath">The path to both envi files including their name. Therefor their names must match.</param>
    /// <param name="datapath">TODO HDF</param>
    /// <exception cref="FileNotFoundException">If either file was found at the specified filepath.</exception>
    /// <exception cref="NotSupportedException">If no envi file, or an ENVI file with a byte order other than the host's was found.</exception>
    public HdfImage(string filepath, string datapath)
    {
        // Trace.WriteLine($"HDF READ {filepath}");

        var mmf = MemoryMappedFile.CreateFromFile(filepath, FileMode.OpenOrCreate, null, 0, MemoryMappedFileAccess.Read);
        var acc = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

        var datafile = H5File.Open(acc);
        var dataset = datafile.Dataset(datapath);

        var width = (long)dataset.Space.Dimensions[1];
        var height = (long)dataset.Space.Dimensions[0];
        var channels = (int)dataset.Space.Dimensions[2];

        Shape = new ImageShape(
            width,
            height,
            channels);

        Layout = new YxzImageMemoryLayout(
            Shape);

        FilePath = filepath;

        FileMapping = mmf;
        FileAccessor = acc;

        Datafile = datafile;
        Dataset = dataset;

        GetValue = dataset.CreateValueGetter<T>();
        GetDoubleValue = dataset.CreateDoubleValueGetter();
        GetDoubleValues = dataset.CreateDoubleValuesGetter();
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

        if (Datafile != null)
        {
            Datafile.Dispose();
            Datafile = null;
        }

        if (FileMapping != null)
        {
            FileMapping.Dispose();
            FileMapping = null;
        }
    }

    public double GetPixel(long x, long y, int channel)
    {
        return GetDoubleValue(x, y, channel);
    }

    public double[] GetPixel(long x, long y)
    {
        //var values = new double[Shape.Channels];
        //for (var i = 0; i < Shape.Channels; i++)
        //{
        //    values[i] = GetPixel(x, y, i);
        //}

        var values = new double[Shape.Channels];

        GetDoubleValues(x, y, Enumerable.Range(0, Shape.Channels).ToArray(), values);

        return values;
    }

    public double[] GetPixel(long x, long y, int[] channels)
    {
        //var values = new double[channels.Length];
        //for (var i = 0; i < channels.Length; i++)
        //{
        //    values[i] = GetPixel(x, y, channels[i]);
        //}

        var values = new double[channels.Length];

        GetDoubleValues(x, y, channels, values);

        return values;
    }

    public LineImage GetPixelLineData(long[] xs, long line, int[] channels)
    {
        // Trace.WriteLine($"HDF GetPixelLineData {xs.Length} {line} {channels.Length} {Path.GetFileNameWithoutExtension(FilePath)}");

        var data = new double[xs.Length][];
        for (var i = 0; i < xs.Length; i++)
        {
            data[i] = new double[channels.Length];
        }

        switch (Layout)
        {
            // checking for memory layout -> better cache accesses
            // iterate over channels last
            case XyzImageMemoryLayout:
            case YxzImageMemoryLayout:
                {
                    //for (var x = 0; x < xs.Length; x++)
                    //{
                    //    for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
                    //    {
                    //        data[x][channelIdx] = GetPixel(xs[x], line, channels[channelIdx]);
                    //    }
                    //}

                    for (var x = 0; x < xs.Length; x++)
                    {
                        GetDoubleValues(xs[x], line, channels, data[x]);
                    }

                    break;
                }
            // iterate over x position last
            case YzxImageMemoryLayout:
            case ZyxImageMemoryLayout:
                {
                    //for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
                    //{
                    //    for (var x = 0; x < xs.Length; x++)
                    //    {
                    //        data[x][channelIdx] = GetPixel(xs[x], line, channels[channelIdx]);
                    //    }
                    //}

                    throw new NotImplementedException(); // break;
                }
            default:
                throw new NotImplementedException($"Efficient pixel data line getter not implemented for  layout type {Layout.GetType()} when using ENVI images");
        }

        return new LineImage(data);
    }

    public LineImage GetPixelLineData(long line, int[] channels)
    {
        var data = new double[Shape.Width][];
        for (var i = 0; i < Shape.Width; i++)
        {
            data[i] = new double[channels.Length];
        }

        switch (Layout)
        {
            // checking for memory layout -> better cache accesses
            // iterate over channels last
            case XyzImageMemoryLayout:
            case YxzImageMemoryLayout:
                {
                    //for (var x = 0; x < Shape.Width; x++)
                    //{
                    //    for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
                    //    {
                    //        data[x][channelIdx] = GetPixel(x, line, channels[channelIdx]);
                    //    }
                    //}

                    for (var x = 0; x < Shape.Width; x++)
                    {
                        GetDoubleValues(x, line, channels, data[x]);
                    }

                    break;
                }
            // iterate over x position last
            case YzxImageMemoryLayout:
            case ZyxImageMemoryLayout:
                {
                    //for (var channelIdx = 0; channelIdx < channels.Length; channelIdx++)
                    //{
                    //    for (var x = 0; x < Shape.Width; x++)
                    //    {
                    //        data[x][channelIdx] = GetPixel(x, line, channels[channelIdx]);
                    //    }
                    //}

                    throw new NotImplementedException(); // break;
                }
            default:
                throw new NotImplementedException($"Efficient pixel data line getter not implemented for  layout type {Layout.GetType()} when using ENVI images");
        }

        return new LineImage(data);
    }

    public T this[long x, long y, int channel] => GetValue(x, y, channel);
}

/// <summary>
/// This class contains the utility to open new ENVI files. 
/// </summary>
public static class HdfImage
{
    /// <summary>
    /// This method opens a new file as an envi file given the supplied path. Its corresponding header file must be located at the same location with the same name.
    /// </summary>
    /// <param name="filepath">The path to the ENVI file (header and data).</param>
    /// <returns> A generic <see cref="EnviImage{T}"/> cast to its superclass <see cref="EnviImage"/> which represents the files found under the supplied path.</returns>
    /// <exception cref="FileNotFoundException">If either the header or the raw file were not found in under the supplied path.</exception>
    public static IMemoryImage OpenImage(string filepath)
    {
        // TODO determine proper data type,
        //      preliminary assume 8-bit
        return new HdfImage<byte>(filepath);
    }
}
