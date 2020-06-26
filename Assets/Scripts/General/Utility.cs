using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains useful utility functions
public static class Utility
{
    /// <summary>
    /// Used to map a value in the range [start1 - end1] to the corresponding value
    /// In the range [start2 - end2]
    /// </summary>
    /// <returns>
    /// A value in the range [start2 - end2] that corresponds to the supplied value
    /// </returns>
    public static float Map(float start1, float end1, float start2, float end2, float value)
    {
        if (value < start1 || value > end1)
        {
            // The supplied value is out of range
            Debug.LogWarning("Out of range value supplied in Map function.");
            return value;
        }
        else if (start1 == end1 || start2 == end2)
        {
            // Prevent division by 0
            Debug.LogWarning("Invalid range supplied in Map function.");
            return value;
        }
        return start2 + (((value - start1) * (end2 - start2)) / (end1 - start1));
    }

    /// <summary>
    /// Returns a copy of a generic array. Could be used when we need to pass an array to a function
    /// while preventing that function from editing the original array.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public static T[] GetCopy<T>(this T[] array)
    {
        T[] copy = new T[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            copy[i] = array[i];
        }
        return copy;
    }

    /// <summary>
    /// Returns a copy of a generic list. Could be used when we need to pass a list to a function
    /// while preventing that function from editing the original list.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public static List<T> GetCopy<T>(this List<T> list)
    {
        List<T> copy = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            copy.Add(list[i]);
        }
        return copy;
    }
}
