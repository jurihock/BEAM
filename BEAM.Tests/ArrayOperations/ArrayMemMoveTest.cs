using BEAM.Image.Threading;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BEAM.Tests.ArrayOperations;

public class ArrayMemMoveTest
{
    [Fact]
    public void SimpleMemoryMovementTest()
    {
        byte[,,] array = new byte[4, 4, 4]
        {
            { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } },
            { { 17, 18, 19, 20 }, { 21, 22, 23, 24 }, { 25, 26, 27, 28 }, { 29, 30, 31, 32 } },
            { { 33, 34, 35, 36 }, { 37, 38, 39, 40 }, { 41, 42, 43, 44 }, { 45, 46, 47, 48 } },
            { { 49, 50, 51, 52 }, { 53, 54, 55, 56 }, { 57, 58, 59, 60 }, { 61, 62, 63, 64 } }
        };
        Console.WriteLine("Init array ----");
        Print3DArray(array);
        Console.WriteLine($"Element origin: {array[1,1,1]}");
        Console.WriteLine($"Element x: {array[3,1,1]}");
        Console.WriteLine($"Element y: {array[1,3,1]}");
        Console.WriteLine($"Element z: {array[1,1,3]}");
        Console.WriteLine($"Element dest: {array[2,2,2]}");
        array.Move3D(1, 1, 1, 2, 2, 2, 2, 2, 2);
        Console.WriteLine("Resulting array ----");
        Print3DArray(array);
    }
    static void Print3DArray(byte[,,] array)
    {
        for (int x = 0; x < array.GetLength(0); x++)
        {
            Console.WriteLine($"Layer {x}:");
            for (int y = 0; y < array.GetLength(1); y++)
            {
                for (int z = 0; z < array.GetLength(2); z++)
                {
                    Console.Write($"{array[x, y, z],2} ");
                }
                Console.WriteLine();
            }
        }
    }
}

