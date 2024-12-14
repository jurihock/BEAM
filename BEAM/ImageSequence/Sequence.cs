using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BEAM.Image;
using BEAM.Models.LoggerModels;

namespace BEAM.ImageSequence;

/// <summary>
/// Loads and manages an entire sequence.
/// </summary>
/// <param name="imagePaths">The paths of the images to use inside the sequence.</param>
public abstract class Sequence(List<string> imagePaths)
{
    /// Do not use -> set internally on first get
    private long? _singleFileHeight;

    private long SingleImageHeight
    {
        get
        {
            if (_singleFileHeight is not null) return _singleFileHeight.Value;
            _singleFileHeight = GetImage(0).Shape.Height;
            return _singleFileHeight.Value;
        }
    }

    /// Do not use -> set internally on first get
    private SequenceShape? _shape
    {
        get
        {
            if (_shape is not null)
            {
                return _shape;
            }
            else
            {
                _InitializeShape();
                return _shape;
            }
        }
        set
        {
            _shape = value;
        }
    }

    /// <summary>
    /// The Shape (width, height, channel count) of the entire sequence
    /// </summary>
    public SequenceShape Shape
    {
        get => _shape;
        /*TODO: remove
         get
        {
            if (_shape is not null) return _shape;
            _InitializeShape();
            return _shape!;
        }*/
    }

    private Mutex _loadedImagesMutex = new();
    private IContiguousImage?[] _loadedImages = new IContiguousImage?[imagePaths.Count];

    /// <summary>
    /// Loads the desired image so that it's data can be accessed randomly.
    /// </summary>
    /// <param name="index">The index of the desired image.</param>
    /// <returns>The loaded image.</returns>
    protected abstract IContiguousImage LoadImage(int index);

    /// <summary>
    /// Initializes and validates the sequence.
    /// </summary>
    protected abstract void InitializeSequence();

    /// <summary>
    /// Returns the desired image. Loads the image into main memory on-demand if necessary.
    /// </summary>
    /// <param name="index">The index of the image to load.</param>
    /// <returns>The image with the correct index.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public IContiguousImage GetImage(int index)
    {
        if (index < 0 || index >= _loadedImages.Length)
        {
            throw new NotImplementedException("Invalid image index");
        }

        var img = _loadedImages[index];
        if (img is not null) return img;

        _loadedImagesMutex.WaitOne();
        img = _loadedImages[index];
        if (img is not null)
        {
            _loadedImagesMutex.ReleaseMutex();
            return img;
        }

        img = LoadImage(index);
        _loadedImages[index] = img;
        _loadedImagesMutex.ReleaseMutex();
        return img;
    }

    /// <summary>
    /// Returns the value of a single channel of a single pixel.
    /// </summary>
    /// <param name="x">The x position of the pixel.</param>
    /// <param name="y">The line of the pixel.</param>
    /// <param name="channel">The channel of the image to look into.</param>
    /// <returns>The pixel value at the desired channel.</returns>
    public double GetPixel(long x, long y, int channel)
    {
        if (x >= Shape.Width || x < 0)
            throw new ArgumentException($"Pixel row out of range -> is: {x}, Range: [0, {Shape.Width})");
        if (y >= Shape.Height || y < 0)
            throw new ArgumentException($"Pixel line out of range -> is: {y}, Range: [0, {Shape.Height})");

        long line = y % SingleImageHeight;
        long imgIndex = y / SingleImageHeight;

        var image = GetImage((int)imgIndex);
        return image.GetAsDouble(x, line, channel);
    }

    /// <summary>
    /// Returns the entire value of a pixel -> all channel values are returned
    /// </summary>
    /// <param name="x">The x position of the pixel.</param>
    /// <param name="y">The line of the pixel.</param>
    /// <returns>The array containing the values of all channels for the pixel. The size of the array is Shape.Channels.</returns>
    public double[] GetPixel(long x, long y)
    {
        if (x >= Shape.Width || x < 0)
            throw new ArgumentException($"Pixel row out of range -> is: {x}, Range: [0, {Shape.Width})");
        if (y >= Shape.Height || y < 0)
            throw new ArgumentException($"Pixel line out of range -> is: {y}, Range: [0, {Shape.Height})");

        long line = y % SingleImageHeight;
        long imgIndex = y / SingleImageHeight;
        var image = GetImage((int)imgIndex);

        var channels = new double[Shape.Channels];
        for (int i = 0; i < channels.Length; i++)
        {
            channels[i] = image.GetAsDouble(x, line, i);
        }

        return channels;
    }

    /// <summary>
    /// Returns the pixel data of an entire line.
    /// </summary>
    /// <param name="y"></param>
    /// <returns>A 2d-array with the pixel channel values. Access: [x position, channel index]</returns>
    public double[,] GetPixelLine(long y)
    {
        if (y >= Shape.Height || y < 0)
            throw new ArgumentException($"Pixel line out of range -> is: {y}, Range: [0, {Shape.Height})");

        long line = y % SingleImageHeight;
        long imgIndex = y / SingleImageHeight;

        var image = GetImage((int)imgIndex);

        var values = new double[Shape.Width, Shape.Channels];
        for (int i = 0; i < Shape.Width; i++)
        {
            for (int j = 0; j < Shape.Channels; j++)
            {
                values[i, j] = image.GetAsDouble(i, line, j);
            }
        }

        return values;
    }

    /// <summary>
    /// Calculates the shape of the entire sequence.
    /// Only run once: when Shape is accessed the first tie
    /// </summary>
    private void _InitializeShape()
    {
        int length = imagePaths.Count;

        var firstImage = GetImage(0);
        long width = firstImage.Shape.Width;
        long height = firstImage.Shape.Height;
        _singleFileHeight = height;
        int channels = firstImage.Shape.Channels;

        if (length == 1)
        {
            _shape = new SequenceShape(width, height, channels);
            return;
        }

        height *= (length - 1);

        var lastImage = GetImage(length - 1);
        height += lastImage.Shape.Height;

        _shape = new SequenceShape(width, height, firstImage.Shape.Channels);
    }

    /// <summary>
    /// The supported file types and their respective Sequence type
    /// </summary>
    private static readonly Dictionary<string, Type> SequenceTypes = new()
    {
        [".png"] = typeof(SkiaSequence),
        [".raw"] = typeof(EnviSequence),
        [".hdr"] = typeof(EnviSequence),
    };

    /// <summary>
    /// Opens a new sequence.
    /// </summary>
    /// <param name="paths">The image paths the sequence uses.</param>
    /// <returns>The sequence</returns>
    /// <exception cref="NotImplementedException">Throws when no images are being passed or all found file extensions are unsupported</exception>
    public static Sequence Open(List<string> paths)
    {
        int skipped = 0;
        if (paths.Count == 0) throw new NotImplementedException("Empty sequences are not supported");

        var extensions = paths.Select(Path.GetExtension).ToHashSet();
        foreach (var extension in extensions.OfType<string>())
        {
            if (!SequenceTypes.TryGetValue(extension, out var type))
            {
                skipped++;
                continue;
            }
            
            var sequence = _InstantiateFromType(type, paths);
            sequence.InitializeSequence();

            return sequence;
        }

        if (skipped > 0)
        {
            string warningString = skipped + " Files could not be loaded";

            Logger? logger = Logger.getInstance("../../../../BEAM.Tests/loggerTests/testLogs/testLog.log");
            logger.Warning(LogEvent.FileNotFound, warningString);
        }

        throw new NotImplementedException($"Unsupported extensions: {string.Join(", ", extensions)}");
    }

    /// <summary>
    /// Opens a sequence from a folder
    /// </summary>
    /// <param name="folder">The folder with the sequence inside</param>
    /// <returns>The opened sequence</returns>
    public static Sequence Open(string folder)
    {
        if (!Directory.Exists(folder)) throw new NotImplementedException($"Unsupported folder: {folder}");
        var filePaths = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        return Open(filePaths.ToList());
    }

    /// <summary>
    /// Instantiates a sequence object from a sequence type and image paths
    /// </summary>
    /// <param name="type">The type of the sequence</param>
    /// <param name="paths">The paths of the used images</param>
    /// <returns>The sequence object</returns>
    private static Sequence _InstantiateFromType(Type type, List<string> paths)
    {
        // opens constructor with List<string> as parameter (main constructor of Sequence)
        var ctor = type.GetConstructor([typeof(List<string>)])!;
        return (Sequence)ctor.Invoke([paths]);
    }
}