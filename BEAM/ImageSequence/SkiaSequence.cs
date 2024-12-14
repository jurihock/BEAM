using System.Collections.Generic;
using System.IO;
using BEAM.Image;
using BEAM.Image.Skia;

namespace BEAM.ImageSequence;

public class SkiaSequence(List<string> imagePaths) : Sequence(imagePaths)
{
    protected override IContiguousImage LoadImage(int index) => new RgbSkiaImage(imagePaths[index]);

    protected override void InitializeSequence()
    {
        imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".png"));
    }
}