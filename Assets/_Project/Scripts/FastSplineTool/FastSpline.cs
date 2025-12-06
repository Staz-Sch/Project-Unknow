using UnityEngine;
using System.Runtime.CompilerServices;

public struct FastSpline
{
    public Vector3 p0, p1, p2, p3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 Evaluate(float t)
    {
        float tt = t * t;
        float ttt = tt * t;
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * tt +
            (-p0 + 3f * p1 - 3f * p2 + p3) * ttt
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 EvaluateTangent(float t)
    {
        float tt = t * t;
        return 0.5f * (
            (-p0 + p2) +
            2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t +
            3f * (-p0 + 3f * p1 - 3f * p2 + p3) * tt
        ).normalized;
    }
}
