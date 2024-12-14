using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BEAM.Image;

namespace BEAM.ImageSequence;

public abstract class Sequence(List<string> imagePaths)
{
    // Do not use -> set internally on first get
    private long? _singleFileHeight;

    public long SingleImageHeight
    {
        get
        {
            if (_singleFileHeight is not null) return _singleFileHeight.Value;
            _singleFileHeight = GetImage(0).Shape.Height;
            return _singleFileHeight.Value;
        }
    }

    // DO not use -> set internally on first get
    private SequenceShape? _shape;

    public SequenceShape Shape
    {
        get
        {
            if (_shape is not null) return _shape;
            _InitializeShape();
            return _shape!;
        }
    }

    private Mutex _loadedImagesMutex = new();
    private IContiguousImage?[] _loadedImages = new IContiguousImage?[imagePaths.Count];

    protected abstract IContiguousImage LoadImage(int index);
    protected abstract void InitializeSequence();

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

    public double GetPixel(long x, long y, int channel)
    {
        long line = y % SingleImageHeight;
        long imgIndex = y / SingleImageHeight;

        var image = GetImage((int) imgIndex);
        return image.GetAsDouble(x, line, channel);
    }

    public double[] GetPixel(long x, long y)
    {
        var channels = new double[Shape.Channels];
        for (int i = 0; i < channels.Length; i++)
        {
            channels[i] = GetPixel(x, y, i);
        }

        return channels;
    }

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

    private static readonly Dictionary<string, Type> SequenceTypes = new()
    {
        [".png"] = typeof(SkiaSequence),
        [".raw"] = typeof(EnviSequence),
        [".hdr"] = typeof(EnviSequence),
    };

    // ReSharper disable once MemberCanBePrivate.Global
    public static Sequence Open(List<string> paths)
    {
        if (paths.Count == 0) throw new NotImplementedException("Empty sequences are not supported");

        var extensions = paths.Select(Path.GetExtension).ToHashSet();
        foreach (var extension in extensions.OfType<string>())
        {
            if (!SequenceTypes.TryGetValue(extension, out var type)) continue;

            var sequence = _InstantiateFromType(type, paths);
            sequence.InitializeSequence();
            return sequence;
        }

        throw new NotImplementedException($"Unsupported extensions: {string.Join(", ", extensions)}");
    }

    public static Sequence Open(string folder)
    {
        var filePaths = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        return Open(filePaths.ToList());
    }

    private static Sequence _InstantiateFromType(Type type, List<string> paths)
    {
        // opens constructor with List<string> as parameter (main constructor of Sequence)
        var ctor = type.GetConstructor([typeof(List<string>)])!;
        return (Sequence)ctor.Invoke([paths]);
    }
}