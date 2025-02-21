using System;
using System.Runtime.InteropServices;

namespace BEAM.Datatypes.Color;

/// <summary>
/// Cass representing an 8-bit BGRA color value (with A being transparency).
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BGRA
{
  public byte B;
  public byte G;
  public byte R;
  public byte A;

  public byte this[int index]
  {
    readonly get
    {
      switch (index)
      {
        case 0: return B;
        case 1: return G;
        case 2: return R;
        case 3: return A;
      }

      throw new ArgumentOutOfRangeException(nameof(index));
    }
    set
    {
      switch (index)
      {
        case 0: B = value; return;
        case 1: G = value; return;
        case 2: R = value; return;
        case 3: A = value; return;
      }

      throw new ArgumentOutOfRangeException(nameof(index));
    }
  }
}
