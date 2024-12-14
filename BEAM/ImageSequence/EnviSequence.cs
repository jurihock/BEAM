using System.Collections.Generic;
using System.IO;
using BEAM.Image;
using BEAM.Image.Envi;

namespace BEAM.ImageSequence;

public class EnviSequence(List<string> imagePaths) : Sequence(imagePaths)
{
    protected override IContiguousImage LoadImage(int index) => EnviImage.OpenImage(imagePaths[index]);

    protected override void InitializeSequence()
    {
        imagePaths.RemoveAll(path => !Path.GetExtension(path).Equals(".raw"));
    }
}