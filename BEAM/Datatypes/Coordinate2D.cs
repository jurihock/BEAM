using System;
using ScottPlot;

namespace BEAM.Datatypes;

/// <summary>
/// An immutable Coordinate pair.
/// </summary>
public readonly struct Coordinate2D : IEquatable<Coordinate2D>
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

    public Coordinate2D OffsetBy(double x, double y)
    {
        return new Coordinate2D(Row + y, Column + x);
    }
    
    public override string ToString()
    {
        return $"Row: {Row}, Column: {Column}";
    }

    public bool Equals(Coordinate2D other)
    {
        return Row.Equals(other.Row) && Column.Equals(other.Column);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Row, Column);
    }
}