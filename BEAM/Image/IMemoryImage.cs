using System;

namespace BEAM.Image;

/// <summary>
/// Interface for an image that can be accessed using a designated memory layout
/// </summary>
public interface IMemoryImage : IImage
{
    /// <summary>
    /// The in-memory layout of the image
    /// </summary>
    ImageMemoryLayout Layout { get; }
}