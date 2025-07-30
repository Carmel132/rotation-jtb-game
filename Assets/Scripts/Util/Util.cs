using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Class for holding static utility functions and variables
/// </summary>
public static class Util
{
    static ContactFilter2D NO_FILTER { get; } = new();

    static Util()
    {
        NO_FILTER.NoFilter();
    }

    /// <summary>
    /// Performs a raycast without a filter. Raycast is used to detect colliders in a linear path
    /// </summary>
    /// <param name="start">The start point of the raycast</param>
    /// <param name="dir">The direction vector of the ray</param>
    /// <param name="distance">The distance the ray travels</param>
    /// <param name="hits">Out variable to store the colliders hit in the path</param>
    /// <returns>The number of colliders hit</returns>
    public static int UnfilteredRaycast2D(Vector3 start, Vector3 dir, float distance, out List<RaycastHit2D> hits)
    {
        hits = new();
        return Physics2D.Raycast(start, dir, NO_FILTER, hits, distance);
    }
    /// <summary>
    /// Same as `UnfilteredRaycast2D` but draws the raycast 
    /// </summary>
    public static int DebugUnfilteredRaycast2D(Vector3 start, Vector3 dir, float distance, out List<RaycastHit2D> hits)
    {
        Debug.DrawRay(start, dir.normalized * distance, Color.red);
        return UnfilteredRaycast2D(start, dir, distance, out hits);
    }
};
