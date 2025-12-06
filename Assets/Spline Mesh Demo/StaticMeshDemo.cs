using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplineRoadBuilder : MonoBehaviour
{
#if ODIN_INSPECTOR
    [Title("Spline Road Generator")]
#endif

    [Header("Spline Source")]
    [SerializeField] private SplineContainer spline;

#if ODIN_INSPECTOR
    [BoxGroup("Settings"), LabelWidth(100)]
    [MinValue(2), MaxValue(500)]
#endif
    [SerializeField] private int resolution = 50;

#if ODIN_INSPECTOR
    [BoxGroup("Settings"), LabelWidth(100)]
#endif
    [SerializeField] private float width = 2f;

#if ODIN_INSPECTOR
    [BoxGroup("Settings"), LabelText("UV Scale"), MinValue(0.01f)]
#endif
    [SerializeField] private float uvScale = 1f;

#if ODIN_INSPECTOR
    [BoxGroup("Settings"), LabelText("Show Gizmos")]
#endif
    [SerializeField] private bool showGizmos = true;

    private Mesh mesh;

    // Used to detect changes when editing spline points
    private Vector3[] lastPositions;

    private void OnEnable()
    {
        if (!mesh)
        {
            mesh = new Mesh { name = "Spline Road Mesh" };
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (spline != null)
                BuildRoad();
        }
#endif
    }

    private void BuildRoad()
    {
        if (spline == null)
            return;

        List<Vector3> verts = new();
        List<Vector2> uvs = new();
        List<int> tris = new();

        float halfWidth = width * 0.5f;
        float step = 1f / resolution;

        float totalLength = spline.CalculateLength();
        float uvDistance = 0f;

        Vector3 prevPos = (Vector3)spline.EvaluatePosition(0f);

        for (int i = 0; i <= resolution; i++)
        {
            float t = i * step;

            Vector3 pos = (Vector3)spline.EvaluatePosition(t);
            Vector3 forward = ((Vector3)spline.EvaluateTangent(t)).normalized;
            Vector3 up = ((Vector3)spline.EvaluateUpVector(t)).normalized;
            Vector3 right = Vector3.Cross(up, forward).normalized;

            Vector3 leftEdge = pos - right * halfWidth;
            Vector3 rightEdge = pos + right * halfWidth;

            verts.Add(leftEdge);
            verts.Add(rightEdge);

            uvDistance += Vector3.Distance(pos, prevPos);
            uvs.Add(new Vector2(0, uvDistance * uvScale));
            uvs.Add(new Vector2(1, uvDistance * uvScale));
            prevPos = pos;

            if (i > 0)
            {
                int baseIndex = i * 2;
                tris.Add(baseIndex - 2);
                tris.Add(baseIndex);
                tris.Add(baseIndex - 1);

                tris.Add(baseIndex);
                tris.Add(baseIndex + 1);
                tris.Add(baseIndex - 1);
            }
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorUtility.SetDirty(mesh);
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmos || spline == null)
            return;

        Gizmos.color = Color.cyan;
        float step = 1f / resolution;
        Vector3 prev = (Vector3)spline.EvaluatePosition(0f);
        for (float t = step; t <= 1f; t += step)
        {
            Vector3 pos = (Vector3)spline.EvaluatePosition(t);
            Gizmos.DrawLine(prev, pos);
            prev = pos;
        }
    }
#endif
}
