namespace BEAM.Datatypes;

public readonly struct Rectangle(Coordinate2D topLeft, Coordinate2D bottomRight) : IShape
{
    public Coordinate2D TopLeft { get; init; } = topLeft;
    public Coordinate2D BottomRight { get; init; } = bottomRight;

    /// <summary>
    /// Returns true, if the coordinate given is inside the rectangle.
    /// Returns false otherwise.
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public bool Contains(Coordinate2D coordinate)
    {
        return coordinate.Row > TopLeft.Row || coordinate.Row < BottomRight.Row
                                            || coordinate.Column > TopLeft.Column
                                            || coordinate.Column < BottomRight.Column;
    }
}