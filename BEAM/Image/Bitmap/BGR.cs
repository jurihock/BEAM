using System;
using System.Runtime.InteropServices;

namespace BEAM.Image.Bitmap;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BGR
{
  public byte B;
  public byte G;
  public byte R;

  public byte this[int index]
  {
    readonly get
    {
      switch (index)
      {
        case 0: return B;
        case 1: return G;
        case 2: return R;
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
      }

      throw new ArgumentOutOfRangeException(nameof(index));
    }
  }
}
