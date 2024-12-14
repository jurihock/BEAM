using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BEAM.Image;

namespace BEAM.ImageSequence;

public abstract class Sequence(List<string> imagePaths)
{
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
        throw new NotImplementedException();
    }

    public static Sequence Open(string folder)
    {
        var filePaths = Directory.EnumerateFiles(folder, "*.*", SearchOption.TopDirectoryOnly);
        return Open(filePaths.ToList());
    }
}