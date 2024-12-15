
namespace BEAM.Image.Bitmap;

public sealed partial class BgraBitmap
{
    private const int BitmapBitsPerPixel = 32;
    private const int BitmapBytesPerPixel = 4;

    private const int BitmapFileHeaderSize = 14;
    private const int BitmapInfoHeaderSize = 40;
    private const int BitmapHeaderSize = BitmapFileHeaderSize + BitmapInfoHeaderSize;
}