using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
public static class Util
{
    static ContactFilter2D NO_FILTER { get; } = new();

    static Util()
    {
        NO_FILTER.NoFilter();
    }

    public static int UnfilteredRaycast2D(Vector3 start, Vector3 dir, float distance, out List<RaycastHit2D> hits)
    {
        hits = new();
        return Physics2D.Raycast(start, dir, NO_FILTER, hits, distance);
    }
};
