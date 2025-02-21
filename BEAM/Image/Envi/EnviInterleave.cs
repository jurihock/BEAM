
namespace BEAM.Image.Envi;

/// <summary>
/// The enum represents different ways to organize pixel data in a file. Their orientation/layout in the filestream is thereby represented.
/// </summary>
public enum EnviInterleave
{
    /// <summary>
    /// YXZ
    /// </summary>
    BIP = 012,

    /// <summary>
    /// YZX
    /// </summary>
    BIL = 021,

    /// <summary>
    /// ZYX
    /// </summary>
    BSQ = 201,
}
