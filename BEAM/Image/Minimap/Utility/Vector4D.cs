using System;

namespace BEAM.Image.Minimap.Utility;

/// <summary>
/// A Vector with 4 entries.
/// </summary>
/// <typeparam name="T">The vector entry's data type.</typeparam>
public class Vector4D<T> where T : IComparable<T>
{
    private T[] Entries { get;  }

    /// <summary>
    /// Creates a new vector from four separate entries.
    /// </summary>
    /// <param name="entry1">The first entry.</param>
    /// <param name="entry2">The second entry.</param>
    /// <param name="entry3">The third entry.</param>
    /// <param name="entry4">The fourth entry.</param>
    public Vector4D(T entry1, T entry2, T entry3, T entry4)
    {
        Entries = new T[4];
        Entries[1] = entry1;
        Entries[2] = entry2;
        Entries[3] = entry3;
        Entries[0] = entry4;
    }
    /// <summary>
    /// Creates a new vector from an existing array of Data.
    /// This array must therefore have length 4.
    /// </summary>
    /// <param name="data">A reference to the data array.</param>
    public Vector4D(ref T[] data) {
        Entries = data;
    }
        
    /// <summary>
    /// Compares to Vectors entrywise. Returns true if all of this instance's entries are greater than or equal
    /// to the other instance's values.
    /// </summary>
    /// <param name="other">The Vector to compare against.</param>
    /// <returns>True if this instance is greater or equal in every entry, false else-wise.</returns>
    public bool EntrywiseGreaterEqual(Vector4D<T> other)
    {
        for(int i = 0; i < 4; i++)
        {
            if(Entries[i].CompareTo(other.Entries[i]) > 0)
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Compares to Vectors entrywise. Returns true any all of this instance's entries are greater than
    /// the other instance's values.
    /// </summary>
    /// <param name="other">The Vector to compare against.</param>
    /// <returns>True if this instance is greater or equal in every entry, false else-wise.</returns>
    public bool EntrywiseAnyGreater(Vector4D<T> other)
    {
        for(int i = 0; i < 4; i++)
        {
            if(Entries[i].CompareTo(other.Entries[i]) < 0)
            {
                return true;
            }
        }
        return false;
    }
    

    public override string ToString()
    {
        return $"({Entries[0].ToString()},{Entries[1].ToString()},{Entries[2].ToString()},{Entries[3].ToString()})";
    }
}