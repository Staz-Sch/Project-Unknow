using UnityEngine;

[ExecuteAlways]
public class SplineVisualizer : MonoBehaviour
{
    [Header("Spline Points")]
    public Transform p0;
    public Transform p1;
    public Transform p2;
    public Transform p3;

    [Header("Draw Settings")]
    [Range(2, 100)] public int resolution = 20;
    public Color splineColor = Color.yellow;
    public float handleSize = 0.1f;

    private void OnDrawGizmos()
    {
        if (p0 == null || p1 == null || p2 == null || p3 == null)
            return;

        // Set up the spline
        FastSpline spline = new FastSpline
        {
            p0 = p0.position,
            p1 = p1.position,
            p2 = p2.position,
            p3 = p3.position
        };

        Gizmos.color = splineColor;

        // Draw control points
        Gizmos.DrawSphere(p0.position, handleSize);
        Gizmos.DrawSphere(p1.position, handleSize);
        Gizmos.DrawSphere(p2.position, handleSize);
        Gizmos.DrawSphere(p3.position, handleSize);

        // Draw spline curve
        Vector3 prev = spline.Evaluate(0f);
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 curr = spline.Evaluate(t);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }
    }
}
