using System.Collections.Generic;
using System.IO;
using System.Linq;
using BEAM.Image;
using BEAM.Image.Hdf;
using BEAM.Models.Log;
using BEAM.Profiling;

namespace BEAM.ImageSequence;

/// <summary>
/// Implementation details for envi images.
/// </summary>
/// <param name="imagePaths">The envi images to use inside the sequence.</param>
public class HdfSequence(List<string> imagePaths, string name) : DiskSequence(imagePaths, name)
{
    protected internal override IImage LoadImage(int index)
    {
        return HdfImage.OpenImage(ImagePaths[index]);
    }

    protected internal override bool InitializeSequence()
    {
        using var _ = Timer.Start("Initialize HDF Sequence");
        var removed = ImagePaths.RemoveAll(path =>
            !Path.GetExtension(path).Equals(".hdf5"));

        var paths = ImagePaths.Select(p => Path.ChangeExtension(p, null)).Distinct().ToList();

        ImagePaths = paths.Select(p => Path.ChangeExtension(p, ".hdf5")).ToList();

        if (removed > 0)
        {
            Logger.GetInstance().Info(LogEvent.Sequence, $"{removed} file(s) were not loaded into the sequence.");
        }

        return true;
    }
}
