using System.Collections.Generic;
using UnityEngine;

namespace LightweightGame.Runtime
{
    internal static class ProceduralTorsoMesh
    {
        public static Mesh Create()
        {
            float[] heights = { -1f, -0.8f, -0.45f, 0f, 0.45f, 0.78f, 1f };
            float[] widths = { 0.72f, 0.83f, 0.77f, 0.82f, 1f, 0.92f, 0.55f };
            float[] depths = { 0.82f, 1f, 0.88f, 0.93f, 0.86f, 0.72f, 0.55f };
            const int sides = 20;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            for (int ring = 0; ring < heights.Length; ring++)
            {
                for (int side = 0; side < sides; side++)
                {
                    float angle = side * Mathf.PI * 2f / sides;
                    vertices.Add(new Vector3(Mathf.Cos(angle) * widths[ring], heights[ring], Mathf.Sin(angle) * depths[ring]));
                }
            }
            for (int ring = 0; ring < heights.Length - 1; ring++)
            {
                for (int side = 0; side < sides; side++)
                {
                    int next = (side + 1) % sides;
                    int a = ring * sides + side;
                    int b = ring * sides + next;
                    int c = (ring + 1) * sides + side;
                    int d = (ring + 1) * sides + next;
                    triangles.Add(a); triangles.Add(c); triangles.Add(b);
                    triangles.Add(b); triangles.Add(c); triangles.Add(d);
                }
            }
            Mesh mesh = new Mesh { name = "ShapedTorso" };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
