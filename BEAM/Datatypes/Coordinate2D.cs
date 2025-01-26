namespace BEAM.Datatypes;

/// <summary>
/// An immutable Coordinate pair.
/// </summary>
/// <param name="row"></param>
/// <param name="column"></param>
public readonly struct Coordinate2D(int row, int column)
{
    public long Row { get; init; } = row;
    public long Column { get; init; } = column;
}