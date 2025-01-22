using System;

namespace BEAM.Image;

public interface IMemoryImage : IImage
{
    /// <summary>
    /// The in-memory layout of the image
    /// </summary>
    ImageMemoryLayout Layout { get; }
}