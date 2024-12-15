using Avalonia;

namespace BEAM.AffTrans;

public class Transformer
{
    float[][] _matrix;
    private Point _shift;
    private float _inverseDeterminant;
    
    public Transformer(float[][] matrix, Point shift)
    {
        _matrix = matrix;
        _shift = shift;
        float det = _matrix[0][0] * _matrix[1][1] - _matrix[0][1] * _matrix[1][0];
        _inverseDeterminant = 1 / det;
    }

    public (long, long) Transform(long originalX, long originalY)
    {
        //reverse the transformation to find the original coordinates of the sequence
        long transformedX = 
            (long)((_matrix[1][1] * originalX + -_matrix[0][1] * originalY) * _inverseDeterminant + _shift.X);
        long transformedY = 
            (long)((-_matrix[1][0] * originalX + _matrix[0][0] * originalY) * _inverseDeterminant + _shift.Y);
        return (transformedX, transformedY);
    }
    
    public long TransformX(long originalX)
    {
        //reverse the transformation to find the original coordinates of the sequence
        long transformedX = 
            (long)(_matrix[1][1] * originalX * _inverseDeterminant + _shift.X);        
        return transformedX;
    }
    
    public long TransformY(long originalY)
    {
        long transformedY = 
            (long)(_matrix[0][0] * originalY * _inverseDeterminant + _shift.Y);
        return transformedY;
    }
    
    
}