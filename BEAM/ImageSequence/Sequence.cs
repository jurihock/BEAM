using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BEAM.Image;

namespace BEAM.ImageSequence;

public abstract class Sequence(List<string> imagePaths)
{
    private static readonly Dictionary<string, Type> SequenceTypes = new()
    {
        [".png"] = typeof(SkiaSequence),
        [".raw"] = typeof(EnviSequence),
        [".hdr"] = typeof(EnviSequence),
    };

    public SequenceShape Shape { get; protected set; }

    protected abstract IContiguousImage LoadImage(int index);

    public double GetPixel(long x, long y, int channel)
    {
        throw new NotImplementedException();
    }

    public double[] GetPixel(long x, long y)
    {
        throw new NotImplementedException();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static Sequence Open(List<string> paths)
    {
        // TODO: set _singleImageHeight, Shape

        var extensions = paths.Select(Path.GetExtension).ToHashSet();
        foreach (var extension in extensions.OfType<string>())
        {
            if (!SequenceTypes.TryGetValue(extension, out var type)) continue;

            return _InstantiateFromType(type, paths);
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