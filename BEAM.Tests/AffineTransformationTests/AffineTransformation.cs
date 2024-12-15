using Avalonia;
using BEAM.AffTrans;
using BEAM.ImageSequence;
using BEAM.Models.LoggerModels;

namespace BEAM.Tests.AffineTransformationTests;

public class AffineTransformation
{
    [Fact]
    public void NoTransformationTest()
    {
        // Arrange
        Point shift = new Point(0, 0);
        double[][] matrix = [[1,0], [0,1]];
        
        Transformer transformer = new Transformer(matrix, shift);
        long x = 10;
        long y = 10;

        // Act
        (long transformedX, long transformedY) result = transformer.Transform(x, y);

        // Assert
        Assert.True(result.transformedX == 10);
        Assert.True(result.transformedY == 10);
    }
    
    [Fact]
    public void ShiftTransformationTest()
    {
        // Arrange
        Point shift = new Point(10, 10);
        double[][] matrix = [[1,0], [0,1]];
        
        Transformer transformer = new Transformer(matrix, shift);
        long x = 10;
        long y = 10;

        // Act
        (long transformedX, long transformedY) result = transformer.Transform(x, y);

        // Assert
        Assert.True(result.transformedX == 0);
        Assert.True(result.transformedY == 0);
    }
    
    [Fact]
    public void MatrixTransformationTest()
    {
        // Arrange
        Point shift = new Point(0, 0);
        double[][] matrix = [[2,0], [0,2]];
        
        Transformer transformer = new Transformer(matrix, shift);
        long x = 10;
        long y = 10;

        // Act
        (long transformedX, long transformedY) result = transformer.Transform(x, y);

        // Assert
        Assert.True(result.transformedX == 5);
        Assert.True(result.transformedY == 5);
    }
    
    [Fact]
    public void ShiftAndMatrixTransformationTest()
    {
        // Arrange
        Point shift = new Point(10, 10);
        double[][] matrix = [[2,0], [0,2]];
        
        Transformer transformer = new Transformer(matrix, shift);
        long x = 20;
        long y = 20;

        // Act
        (long transformedX, long transformedY) result = transformer.Transform(x, y);

        // Assert
        Assert.True(result.transformedX == 0);
        Assert.True(result.transformedY == 0);
    }
    
    [Fact]
    public void RealisticTransformationTest()
    {

        // Arrange
        Point shift = new Point(2000, 0);
        double[][] matrix = [[2,0], [0,12]];
        
        Transformer transformer = new Transformer(matrix, shift);
        long x = 5000;
        long y = 36000;

        // Act
        (long transformedX, long transformedY) result = transformer.Transform(x, y);

        // Assert
        Assert.True(result.transformedX == 500);
        Assert.True(result.transformedY == 3000);
    }
    
    [Fact]
    public void DifficultTransformationTest()
    {
        // Arrange
        Point shift = new Point(420, 69);
        double[][] matrix = [[3.141, 5.926], [5.358, 9.793]];
        
        Transformer transformer = new Transformer(matrix, shift);
        long x = 7843;
        long y = 99732;

        // Act
        (long transformedX, long transformedY) result = transformer.Transform(x, y);

        // Assert
        Assert.True(result.transformedX == 518512 - 420);
        Assert.True(result.transformedY == -273506 - 69);
    }
}