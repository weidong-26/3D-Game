using System.Collections.Generic;
using UnityEngine;

namespace LightweightGame.Runtime
{
    internal static class ProceduralHeadMesh
    {
        private const int LatitudeSegments = 12;
        private const int LongitudeSegments = 16;

        public static Mesh Create()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            for (int latitude = 0; latitude <= LatitudeSegments; latitude++)
            {
                float v = latitude / (float)LatitudeSegments;
                float phi = Mathf.PI * v;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);

                for (int longitude = 0; longitude <= LongitudeSegments; longitude++)
                {
                    float u = longitude / (float)LongitudeSegments;
                    float theta = Mathf.PI * 2f * u;
                    vertices.Add(new Vector3(
                        Mathf.Cos(theta) * sinPhi * 0.31f,
                        cosPhi * 0.40f,
                        Mathf.Sin(theta) * sinPhi * 0.285f));
                    uvs.Add(new Vector2(u, v));
                }
            }

            int row = LongitudeSegments + 1;
            for (int latitude = 0; latitude < LatitudeSegments; latitude++)
            {
                for (int longitude = 0; longitude < LongitudeSegments; longitude++)
                {
                    int current = latitude * row + longitude;
                    int next = current + row;
                    triangles.Add(current);
                    triangles.Add(next + 1);
                    triangles.Add(next);
                    triangles.Add(current);
                    triangles.Add(current + 1);
                    triangles.Add(next + 1);
                }
            }

            Mesh mesh = new Mesh { name = "UnifiedProceduralHead" };
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            AddMorphs(mesh, vertices);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void AddMorphs(Mesh mesh, List<Vector3> vertices)
        {
            AddMorph(mesh, "FaceWidth", vertices, delegate(Vector3 v) { return new Vector3(v.x * 0.18f, 0f, 0f); });
            AddMorph(mesh, "FaceLength", vertices, delegate(Vector3 v) { return new Vector3(0f, v.y * 0.16f, 0f); });
            AddMorph(mesh, "ChinWidth", vertices, delegate(Vector3 v) { float w = Lower(v); return new Vector3(v.x * 0.30f * w, 0f, 0f); });
            AddMorph(mesh, "ChinLength", vertices, delegate(Vector3 v) { float w = Lower(v); return new Vector3(0f, -0.10f * w, 0f); });
            AddMorph(mesh, "Cheekbones", vertices, delegate(Vector3 v) { float w = Region(v, -0.025f, 0.18f); return new Vector3(Mathf.Sign(v.x) * 0.05f * w, 0f, 0.01f * w); });
            AddMorph(mesh, "EyeSize", vertices, delegate(Vector3 v) { float w = Region(v, 0.09f, 0.17f); return new Vector3(v.x * 0.05f * w, v.y * 0.04f * w, v.z * 0.06f * w); });
            AddMorph(mesh, "EyeSpacing", vertices, delegate(Vector3 v) { float w = Region(v, 0.09f, 0.18f); return new Vector3(Mathf.Sign(v.x) * 0.035f * w, 0f, 0f); });
            AddMorph(mesh, "NoseSize", vertices, delegate(Vector3 v) { float w = Region(v, 0f, 0.12f); return new Vector3(0f, 0f, 0.055f * w); });
            AddMorph(mesh, "NoseWidth", vertices, delegate(Vector3 v) { float w = Region(v, 0f, 0.10f); return new Vector3(v.x * 0.10f * w, 0f, 0f); });
            AddMorph(mesh, "MouthSize", vertices, delegate(Vector3 v) { float w = Region(v, -0.13f, 0.13f); return new Vector3(v.x * 0.10f * w, 0f, 0f); });
            AddMorph(mesh, "LipThickness", vertices, delegate(Vector3 v) { float w = Region(v, -0.13f, 0.07f); return new Vector3(0f, Mathf.Sign(v.y + 0.13f) * 0.02f * w, 0.015f * w); });
        }

        private static void AddMorph(Mesh mesh, string name, List<Vector3> vertices, System.Func<Vector3, Vector3> delta)
        {
            Vector3[] offsets = new Vector3[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                offsets[i] = delta(vertices[i]);
            mesh.AddBlendShapeFrame(name, 100f, offsets, new Vector3[offsets.Length], new Vector3[offsets.Length]);
        }

        private static float Lower(Vector3 vertex)
        {
            return Mathf.Clamp01((-vertex.y + 0.02f) / 0.38f);
        }

        private static float Region(Vector3 vertex, float centerY, float radius)
        {
            if (vertex.z <= 0f)
                return 0f;
            float y = 1f - Mathf.Clamp01(Mathf.Abs(vertex.y - centerY) / radius);
            return y * Mathf.Clamp01(vertex.z / 0.24f);
        }
    }
}
