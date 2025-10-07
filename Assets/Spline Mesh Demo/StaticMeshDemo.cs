using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MultiSplineRoadBuilder : MonoBehaviour
{
#if ODIN_INSPECTOR
    [Title("Spline Road Builder (Seamless)")]
    [InfoBox("Generates a continuous road mesh for all splines in the container.\n• Auto-welds seams\n• Even UV tiling\n• Live edit in Scene")]
#endif
    [Header("Spline Source")]
    [SerializeField, Required] private SplineContainer container;

#if ODIN_INSPECTOR
    [BoxGroup("Road Settings"), LabelText("Width (m)"), MinValue(0.1f)]
#endif
    [SerializeField] private float width = 3f;

#if ODIN_INSPECTOR
    [BoxGroup("Road Settings"), LabelText("Samples per meter"), MinValue(0.1f), MaxValue(10f)]
#endif
    [SerializeField] private float samplesPerMeter = 2f;

#if ODIN_INSPECTOR
    [BoxGroup("UVs"), LabelText("Tiling per meter"), MinValue(0.01f)]
#endif
    [SerializeField] private float vTilesPerMeter = 0.25f;

#if ODIN_INSPECTOR
    [BoxGroup("Debug"), LabelText("Show Gizmos")]
#endif
    [SerializeField] private bool showGizmos = true;

    private Mesh mesh;

    // ---------------------------------------------------------------------
    private void OnEnable()
    {
        if (!mesh)
        {
            mesh = new Mesh { name = "SeamlessRoad" };
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }
        Rebuild();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            Rebuild();
#endif
    }

    // ---------------------------------------------------------------------
#if ODIN_INSPECTOR
    [Button(ButtonSizes.Large), GUIColor(0.3f, 0.8f, 1f)]
#endif
    public void Rebuild()
    {
        if (container == null || container.Splines.Count == 0)
            return;

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        float4x4 l2w = (float4x4)container.transform.localToWorldMatrix;

        float halfW = width * 0.5f;
        float vAccum = 0f;
        int splineOffset = 0;
        Vector3 prevEndLeft = Vector3.zero, prevEndRight = Vector3.zero;
        bool hasPrevEnd = false;

        foreach (var spline in container.Splines)
        {
            float length = SplineUtility.CalculateLength(spline, l2w);
            int sampleCount = Mathf.Max(4, Mathf.CeilToInt(length * samplesPerMeter));
            float step = 1f / sampleCount;

            spline.Evaluate(0f, out float3 f3Start, out _, out _);
            Vector3 prevPos = container.transform.TransformPoint((Vector3)f3Start);

            for (int i = 0; i <= sampleCount; i++)
            {
                float t = i * step;
                spline.Evaluate(t, out float3 f3Pos, out float3 f3Tan, out float3 f3Up);

                Vector3 pos = container.transform.TransformPoint((Vector3)f3Pos);
                Vector3 forward = container.transform.TransformDirection((Vector3)f3Tan).normalized;
                Vector3 up = container.transform.TransformDirection((Vector3)f3Up).normalized;
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Vector3 left = pos - right * halfW;
                Vector3 rightV = pos + right * halfW;

                // --- Weld seam to previous spline
                if (i == 0 && hasPrevEnd)
                {
                    left = prevEndLeft;
                    rightV = prevEndRight;
                }

                verts.Add(left);
                verts.Add(rightV);

                vAccum += Vector3.Distance(pos, prevPos);
                uvs.Add(new Vector2(0f, vAccum * vTilesPerMeter));
                uvs.Add(new Vector2(1f, vAccum * vTilesPerMeter));

                if (i > 0)
                {
                    int bi = splineOffset + i * 2;
                    tris.Add(bi - 2);
                    tris.Add(bi);
                    tris.Add(bi - 1);

                    tris.Add(bi);
                    tris.Add(bi + 1);
                    tris.Add(bi - 1);
                }

                prevPos = pos;
            }

            // remember last pair for next spline
            prevEndLeft = verts[^2];
            prevEndRight = verts[^1];
            hasPrevEnd = true;
            splineOffset = verts.Count;
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorUtility.SetDirty(mesh);
#endif
    }

    // ---------------------------------------------------------------------
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showGizmos || container == null) return;

        Gizmos.color = Color.cyan;

        float4x4 l2w = (float4x4)container.transform.localToWorldMatrix;
        foreach (var spline in container.Splines)
        {
            spline.Evaluate(0f, out float3 f3Start, out _, out _);
            Vector3 prev = container.transform.TransformPoint((Vector3)f3Start);
            const float step = 0.02f;
            for (float t = step; t <= 1f; t += step)
            {
                spline.Evaluate(t, out float3 f3Pos, out _, out _);
                Vector3 p = container.transform.TransformPoint((Vector3)f3Pos);
                Gizmos.DrawLine(prev, p);
                prev = p;
            }
        }
    }
#endif
}
