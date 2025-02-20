using ScottPlot;

namespace BEAM.Datatypes;

/// <summary>
/// An immutable Coordinate pair.
/// </summary>
/// <param name="row"></param>
/// <param name="column"></param>
public readonly struct Coordinate2D
{
    public double Row { get; init; }
    public double Column { get; init; }
    
    public Coordinate2D(int row, int column)
    {
        Row = row;
        Column = column;
    }
   
    public Coordinate2D(long row, long column)
        {
            Row = row;
            Column = column;
        }
    


    public Coordinate2D(double row, double column)
    {
        Row = row;
        Column = column;
    }
    

    public Coordinate2D(Coordinates coordinates)
    {
        Row = coordinates.Y;
        Column = coordinates.X;
    }
    
    public override string ToString()
    {
        return $"Row: {Row}, Column: {Column}";
    }
}