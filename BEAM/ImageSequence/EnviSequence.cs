using System.Collections.Generic;
using BEAM.Image;

namespace BEAM.ImageSequence;

public class EnviSequence(List<string> imagePaths) : Sequence(imagePaths)
{
    protected override IContiguousImage LoadImage(int index)
    {
        throw new System.NotImplementedException();
    }
}