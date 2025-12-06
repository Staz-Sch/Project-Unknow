using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RailDrawer : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera sceneCamera;
    public LayerMask surfaceLayer;
    public float railWidth = 0.3f;
    public float sampleSpacing = 0.2f;

    [Header("Debug")]
    public bool drawGizmos = true;

    private List<Vector3> controlPoints = new();
    private Mesh mesh;

    private void Start()
    {
        if (!sceneCamera)
            sceneCamera = Camera.main;

        mesh = new Mesh { name = "Rail Mesh" };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(sceneCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200f, surfaceLayer))
            {
                controlPoints.Add(hit.point);
                if (controlPoints.Count >= 4)
                    BuildRail();
            }
        }
    }

    private void BuildRail()
    {
        if (controlPoints.Count < 4)
            return;

        List<Vector3> verts = new();
        List<int> tris = new();
        List<Vector2> uvs = new();

        float halfW = railWidth * 0.5f;
        int triOffset = 0;

        // Loop over each segment of 4 points
        for (int i = 0; i < controlPoints.Count - 3; i++)
        {
            FastSpline spline = new FastSpline
            {
                p0 = controlPoints[i],
                p1 = controlPoints[i + 1],
                p2 = controlPoints[i + 2],
                p3 = controlPoints[i + 3]
            };

            Vector3 prevPos = spline.Evaluate(0);
            Vector3 prevForward = spline.EvaluateTangent(0);
            Vector3 up = Vector3.up; // for now, use world up — can be replaced with surface normals

            for (float t = sampleSpacing; t <= 1f; t += sampleSpacing)
            {
                Vector3 pos = spline.Evaluate(t);
                Vector3 forward = spline.EvaluateTangent(t);
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Vector3 left = pos - right * halfW;
                Vector3 rightV = pos + right * halfW;

                verts.Add(left);
                verts.Add(rightV);
                uvs.Add(new Vector2(0, verts.Count / 2f));
                uvs.Add(new Vector2(1, verts.Count / 2f));

                if (verts.Count >= 4)
                {
                    tris.Add(triOffset + 0);
                    tris.Add(triOffset + 2);
                    tris.Add(triOffset + 1);

                    tris.Add(triOffset + 1);
                    tris.Add(triOffset + 2);
                    tris.Add(triOffset + 3);
                    triOffset += 2;
                }

                prevPos = pos;
                prevForward = forward;
            }
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || controlPoints.Count < 2) return;
        Gizmos.color = Color.yellow;
        for (int i = 1; i < controlPoints.Count; i++)
            Gizmos.DrawLine(controlPoints[i - 1], controlPoints[i]);
    }
}
