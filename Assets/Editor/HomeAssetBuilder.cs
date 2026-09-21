using System.Collections.Generic;
using System.IO;
using LightweightGame.Interaction;
using LightweightGame.Runtime;
using UnityEditor;
using UnityEngine;

namespace LightweightGame.Editor
{
    internal static class HomeAssetBuilder
    {
        private const string Materials = "Assets/Art/Materials/Generated";
        private const string Meshes = "Assets/Art/Buildings/Meshes";
        private const string Modules = "Assets/Prefabs/Buildings/Modules";
        private const string Furniture = "Assets/Prefabs/Furniture";

        public static void Prepare()
        {
            Folder("Assets/Art/Buildings/Meshes");
            Folder(Materials);
            Folder(Modules);
            Folder(Furniture);
            Folder("Assets/Prefabs/Interactables");
            Folder("Assets/Art/Environment");
        }

        private static void Folder(string path)
        {
            string[] parts = path.Split('/');
            string parent = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string current = parent + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(current)) AssetDatabase.CreateFolder(parent, parts[i]);
                parent = current;
            }
        }

        public static Material Solid(string name, Color color, float metallic = 0f, float smoothness = 0.4f)
        {
            string path = Materials + "/" + name + ".mat";
            Material result = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (result != null) return result;
            result = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            result.name = name;
            result.SetColor("_BaseColor", color);
            result.SetFloat("_Metallic", metallic);
            result.SetFloat("_Smoothness", smoothness);
            AssetDatabase.CreateAsset(result, path);
            return result;
        }

        public static Material Surface(string name, string sourceFolder, string prefix, Color tint, float metallic = 0f)
        {
            string path = Materials + "/" + name + ".mat";
            Material result = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (result != null) return result;
            result = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            result.name = name;
            result.SetColor("_BaseColor", tint);
            string diffuse = First(sourceFolder, prefix + "_diff_1k.jpg", prefix + "_1K-JPG_Color.jpg");
            string normal = First(sourceFolder, prefix + "_nor_dx_1k.jpg", prefix + "_1K-JPG_NormalDX.jpg");
            string rough = First(sourceFolder, prefix + "_rough_1k.jpg", prefix + "_roughness_1k.jpg", prefix + "_1K-JPG_Roughness.jpg");
            string metal = First(sourceFolder, prefix + "_metal_1k.jpg", prefix + "_metallic_1k.jpg");
            Texture2D baseMap = AssetDatabase.LoadAssetAtPath<Texture2D>(diffuse);
            Texture2D normalMap = LoadNormal(normal);
            if (baseMap != null) result.SetTexture("_BaseMap", baseMap);
            if (normalMap != null)
            {
                result.SetTexture("_BumpMap", normalMap);
                result.EnableKeyword("_NORMALMAP");
            }
            Texture2D packed = PackMetalSmooth(name, rough, metal, metallic);
            if (packed != null)
            {
                result.SetTexture("_MetallicGlossMap", packed);
                result.SetFloat("_Metallic", 1f);
                result.SetFloat("_Smoothness", 1f);
                result.EnableKeyword("_METALLICGLOSSMAP");
            }
            AssetDatabase.CreateAsset(result, path);
            return result;
        }

        private static string First(string folder, params string[] names)
        {
            foreach (string name in names)
            {
                string path = folder + "/" + name;
                if (File.Exists(path)) return path;
            }
            return null;
        }

        private static Texture2D LoadNormal(string path)
        {
            if (path == null) return null;
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.NormalMap)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Texture2D Readable(string path)
        {
            if (path == null) return null;
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Texture2D PackMetalSmooth(string name, string roughPath, string metalPath, float metalConstant)
        {
            if (roughPath == null) return null;
            string path = Materials + "/" + name + "_MetalSmooth.png";
            Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (existing != null) return existing;
            Texture2D rough = Readable(roughPath);
            Texture2D metal = Readable(metalPath);
            Color32[] roughPixels = rough.GetPixels32();
            Color32[] metalPixels = metal != null && metal.width == rough.width && metal.height == rough.height ? metal.GetPixels32() : null;
            Color32[] packedPixels = new Color32[roughPixels.Length];
            byte fixedMetal = (byte)Mathf.RoundToInt(Mathf.Clamp01(metalConstant) * 255f);
            for (int i = 0; i < packedPixels.Length; i++)
                packedPixels[i] = new Color32(metalPixels != null ? metalPixels[i].r : fixedMetal, 0, 0, (byte)(255 - roughPixels[i].r));
            Texture2D result = new Texture2D(rough.width, rough.height, TextureFormat.RGBA32, true, true);
            result.SetPixels32(packedPixels);
            result.Apply();
            File.WriteAllBytes(Path.GetFullPath(path), result.EncodeToPNG());
            Object.DestroyImmediate(result);
            AssetDatabase.ImportAsset(path);
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        public static GameObject Block(string name, Transform parent, Vector3 center, Vector3 size, Material material, bool collider = true, float round = 0f)
        {
            string key = material.name + "_" + size.x.ToString("0.###") + "_" + size.y.ToString("0.###") + "_" + size.z.ToString("0.###") + "_" + round.ToString("0.###") + (collider ? "_C" : "_V");
            string path = Modules + "/" + key + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Mesh mesh = MeshAsset(key, size, round);
                GameObject source = new GameObject(key, typeof(MeshFilter), typeof(MeshRenderer));
                source.GetComponent<MeshFilter>().sharedMesh = mesh;
                source.GetComponent<MeshRenderer>().sharedMaterial = material;
                if (collider) source.AddComponent<BoxCollider>().size = size;
                prefab = PrefabUtility.SaveAsPrefabAsset(source, path);
                Object.DestroyImmediate(source);
            }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = center;
            return instance;
        }

        private static Mesh MeshAsset(string key, Vector3 size, float round)
        {
            string path = Meshes + "/" + key + ".asset";
            Mesh result = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (result != null) return result;
            result = new Mesh { name = key };
            if (round > 0f) Rounded(result, size, round);
            else Box(result, size);
            AssetDatabase.CreateAsset(result, path);
            return result;
        }

        private static void Box(Mesh mesh, Vector3 size)
        {
            Vector3 h = size * 0.5f;
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();
            Face(new Vector3(0, 0, h.z), new Vector3(h.x, 0, 0), new Vector3(0, h.y, 0));
            Face(new Vector3(0, 0, -h.z), new Vector3(-h.x, 0, 0), new Vector3(0, h.y, 0));
            Face(new Vector3(h.x, 0, 0), new Vector3(0, 0, -h.z), new Vector3(0, h.y, 0));
            Face(new Vector3(-h.x, 0, 0), new Vector3(0, 0, h.z), new Vector3(0, h.y, 0));
            Face(new Vector3(0, h.y, 0), new Vector3(h.x, 0, 0), new Vector3(0, 0, -h.z));
            Face(new Vector3(0, -h.y, 0), new Vector3(h.x, 0, 0), new Vector3(0, 0, h.z));
            void Face(Vector3 c, Vector3 u, Vector3 v)
            {
                int start = vertices.Count;
                Vector3 normal = Vector3.Cross(u, v).normalized;
                vertices.Add(c - u - v); vertices.Add(c + u - v); vertices.Add(c + u + v); vertices.Add(c - u + v);
                for (int i = 0; i < 4; i++) normals.Add(normal);
                float width = u.magnitude * 2f, height = v.magnitude * 2f;
                uvs.Add(Vector2.zero); uvs.Add(new Vector2(width, 0)); uvs.Add(new Vector2(width, height)); uvs.Add(new Vector2(0, height));
                triangles.Add(start); triangles.Add(start + 1); triangles.Add(start + 2);
                triangles.Add(start); triangles.Add(start + 2); triangles.Add(start + 3);
            }
            mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetUVs(0, uvs); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
        }

        private static void Rounded(Mesh mesh, Vector3 size, float radius)
        {
            Vector3 h = size * 0.5f;
            radius = Mathf.Min(radius, h.x * 0.8f, h.y * 0.8f, h.z * 0.8f);
            Vector3 inner = h - Vector3.one * radius;
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();
            Face(new Vector3(0, 0, h.z), new Vector3(h.x, 0, 0), new Vector3(0, h.y, 0));
            Face(new Vector3(0, 0, -h.z), new Vector3(-h.x, 0, 0), new Vector3(0, h.y, 0));
            Face(new Vector3(h.x, 0, 0), new Vector3(0, 0, -h.z), new Vector3(0, h.y, 0));
            Face(new Vector3(-h.x, 0, 0), new Vector3(0, 0, h.z), new Vector3(0, h.y, 0));
            Face(new Vector3(0, h.y, 0), new Vector3(h.x, 0, 0), new Vector3(0, 0, -h.z));
            Face(new Vector3(0, -h.y, 0), new Vector3(h.x, 0, 0), new Vector3(0, 0, h.z));
            void Face(Vector3 c, Vector3 u, Vector3 v)
            {
                int start = vertices.Count;
                float[] us = { -1f, -1f + radius / u.magnitude, 1f - radius / u.magnitude, 1f };
                float[] vs = { -1f, -1f + radius / v.magnitude, 1f - radius / v.magnitude, 1f };
                for (int y = 0; y < 4; y++)
                    for (int x = 0; x < 4; x++)
                    {
                        Vector3 raw = c + u * us[x] + v * vs[y];
                        Vector3 clamped = new Vector3(Mathf.Clamp(raw.x, -inner.x, inner.x), Mathf.Clamp(raw.y, -inner.y, inner.y), Mathf.Clamp(raw.z, -inner.z, inner.z));
                        Vector3 normal = (raw - clamped).normalized;
                        vertices.Add(clamped + normal * radius);
                        normals.Add(normal);
                        uvs.Add(new Vector2((us[x] + 1f) * u.magnitude, (vs[y] + 1f) * v.magnitude));
                    }
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                    {
                        int a = start + y * 4 + x, b = a + 1, d = a + 4, e = d + 1;
                        triangles.Add(a); triangles.Add(b); triangles.Add(e);
                        triangles.Add(a); triangles.Add(e); triangles.Add(d);
                    }
            }
            mesh.SetVertices(vertices); mesh.SetNormals(normals); mesh.SetUVs(0, uvs); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
        }

        public static GameObject Model(string id, Transform parent, Vector3 position, Quaternion rotation)
        {
            string path = Furniture + "/" + id + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                string folder = "Assets/Art/Furniture/PolyHaven/" + id;
                GameObject imported = AssetDatabase.LoadAssetAtPath<GameObject>(folder + "/" + id + "_1k.fbx");
                if (imported == null) throw new FileNotFoundException("Missing furniture model", id);
                GameObject root = new GameObject(id);
                GameObject mesh = (GameObject)PrefabUtility.InstantiatePrefab(imported);
                mesh.transform.SetParent(root.transform, false);
                foreach (Renderer r in mesh.GetComponentsInChildren<Renderer>())
                {
                    Material[] assigned = r.sharedMaterials;
                    for (int i = 0; i < assigned.Length; i++)
                    {
                        string sourceName = assigned[i].name;
                        if (sourceName == "Lit" || sourceName.Contains("glass"))
                            assigned[i] = Solid("LampGlass", new Color(0.92f, 0.89f, 0.76f), 0f, 0.85f);
                        else
                            assigned[i] = Surface(sourceName, folder, sourceName, Color.white);
                    }
                    r.sharedMaterials = assigned;
                }
                Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
                bool first = true;
                foreach (Renderer r in mesh.GetComponentsInChildren<Renderer>())
                {
                    if (first) { bounds = r.bounds; first = false; } else bounds.Encapsulate(r.bounds);
                }
                BoxCollider collider = root.AddComponent<BoxCollider>();
                collider.center = bounds.center;
                collider.size = bounds.size;
                prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
                Object.DestroyImmediate(root);
            }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            return instance;
        }
    }
}
