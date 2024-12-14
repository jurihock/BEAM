using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BEAM.Image;

namespace BEAM.ImageSequence;

public abstract class Sequence(List<string> imagePaths)
{
    // Do not use -> set internally on first get
    private long? _singleFileHeight;

    public long SingleFileHeight
    {
        get
        {
            if (_singleFileHeight is not null) return _singleFileHeight.Value;
            _singleFileHeight = LoadImage(0).Shape.Height;
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

    protected abstract IContiguousImage LoadImage(int index);
    protected abstract void InitializeSequence();

    public double GetPixel(long x, long y, int channel)
    {
        throw new NotImplementedException();
    }

    public double[] GetPixel(long x, long y)
    {
        throw new NotImplementedException();
    }

    private void _InitializeShape()
    {
        int length = imagePaths.Count;

        var firstImage = LoadImage(0);
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

        var lastImage = LoadImage(length - 1);
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