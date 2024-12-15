using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BEAM.Image.Threading;

public static class ArrayUtility
{
    [DllImport("msvcrt.dll", EntryPoint = "memmove", SetLastError = false)]
    private static extern IntPtr MemMove(IntPtr dest, IntPtr src, UIntPtr count);
    public static unsafe void Move3D<T>(this T[,,] array, long fromX, long fromY, long fromZ, long chunkSizeX, long chunkSizeY,
        long chunkSizeZ, long toX, long toY, long toZ)
    {
        
        GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
        try
        {
            IntPtr basePtr = handle.AddrOfPinnedObject();
            long dimXLength = array.GetLength(0);
            long dimYLength = array.GetLength(1);
            long dimZLength = array.GetLength(2);

            long actualChunkSizeX = Math.Min(Math.Min(chunkSizeX, dimXLength - toX), dimXLength - fromX);
            long actualChunkSizeY = Math.Min(Math.Min(chunkSizeY, dimYLength - toY), dimYLength - fromY);
            long actualChunkSizeZ = Math.Min(Math.Min(chunkSizeZ, dimZLength - toZ), dimZLength - fromZ);

            long originOffset = (fromX * dimYLength * dimZLength) + (fromY * dimZLength) + fromZ;
            long destination = (toX * dimYLength * dimZLength) + (toY * dimZLength) + toZ;

            long totalElements = actualChunkSizeX * actualChunkSizeY * actualChunkSizeZ;
            int elementSize = sizeof(T); // Each entry is a single byte
            long totalBytes = totalElements * elementSize;

            long xStride = dimYLength * dimZLength; // Number of elements in 1 "x-layer"
            long yStride = dimZLength;
            int byteOffset = (int)((actualChunkSizeX * xStride + actualChunkSizeY * yStride + actualChunkSizeZ) *
                                   elementSize);
            
            if (byteOffset < 0)
            {
                byteOffset = (int)((byteOffset % totalBytes + totalBytes) % totalBytes);
            }
            else
            {
                byteOffset %= (int) totalBytes;
            }
            
            MemMove(basePtr + byteOffset, basePtr, new UIntPtr((uint)totalBytes));
        }
        catch (Exception exception)
        {
            //CriticalAvoloniaException();
        }
        finally
        {
            handle.Free();
        }

    }
}