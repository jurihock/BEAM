using System;

namespace BEAM.ImageSequence;

public class SequenceShape(long width, long height, int channels) : IEquatable<SequenceShape>
{
    /// <summary>
    /// Number of available image pixels.
    /// </summary>
    public long Width { get; } = width;

    /// <summary>
    /// Number of available image lines.
    /// </summary>
    public long Height { get; } = height;

    /// <summary>
    /// Number of available image color channels.
    /// </summary>
    public int Channels { get; } = channels;

    public SequenceShape(SequenceShape other) : this(other.Width, other.Height, other.Channels)
    {
    }

    public static bool operator ==(SequenceShape left, SequenceShape right) => left.Equals(right);
    public static bool operator !=(SequenceShape left, SequenceShape right) => !left.Equals(right);

    public bool Equals(SequenceShape? other)
    {
        if (other is null) return false;

        if (this.Width != other.Width)
        {
            return false;
        }

        if (this.Height != other.Height)
        {
            return false;
        }

        if (this.Channels != other.Channels)
        {
            return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is SequenceShape other)
        {
            return other.Equals(this);
        }

        return false;
    }

    public override string ToString()
    {
        return $"{Width}x{Height}x{Channels}";
    }

    public override int GetHashCode()
    {
        return $"{Width}x{Height}x{Channels}".GetHashCode();
    }
}