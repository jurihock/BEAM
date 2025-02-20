namespace BEAM.Image.Bitmap;

public sealed partial class BgrBitmap
{
  private const int BitmapBitsPerPixel = 24;
  private const int BitmapBytesPerPixel = 3;

  private const int BitmapFileHeaderSize = 14;
  private const int BitmapInfoHeaderSize = 40;
  private const int BitmapHeaderSize = BitmapFileHeaderSize + BitmapInfoHeaderSize;
}
