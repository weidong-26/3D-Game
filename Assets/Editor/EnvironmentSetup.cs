using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LightweightGame.Editor
{
    public static class EnvironmentSetup
    {
        public static void ConfigureAndReport()
        {
            ConfigureUrp();
            ReportModels();
        }

        public static void ConfigureUrp()
        {
            const string folder = "Assets/Art/Materials";
            const string rendererPath = folder + "/HomeRenderer.asset";
            const string pipelinePath = folder + "/HomeURP.asset";
            if (!AssetDatabase.IsValidFolder("Assets/Art")) AssetDatabase.CreateFolder("Assets", "Art");
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets/Art", "Materials");

            UniversalRendererData renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
            if (renderer == null)
            {
                renderer = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(renderer, rendererPath);
            }
            UniversalRenderPipelineAsset pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
            if (pipeline == null)
            {
                pipeline = UniversalRenderPipelineAsset.Create(renderer);
                AssetDatabase.CreateAsset(pipeline, pipelinePath);
            }
            GraphicsSettings.defaultRenderPipeline = pipeline;
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipeline;
            }
            QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1, false);
            EditorUtility.SetDirty(pipeline);
            AssetDatabase.SaveAssets();
            Debug.Log("HOME_URP_CONFIGURED " + pipelinePath);
        }

        public static void ReportModels()
        {
            string[] ids = { "modern_arm_chair_01", "modern_coffee_table_01", "modern_wooden_cabinet", "drawer_cabinet", "dining_chair_02", "side_table_01", "modern_ceiling_lamp_01" };
            foreach (string id in ids)
            {
                string path = "Assets/Art/Furniture/PolyHaven/" + id + "/" + id + "_1k.fbx";
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (source == null) { Debug.LogError("HOME_MODEL_MISSING " + path); continue; }
                GameObject instance = Object.Instantiate(source);
                Bounds bounds = new Bounds(instance.transform.position, Vector3.zero);
                bool first = true;
                foreach (Renderer r in instance.GetComponentsInChildren<Renderer>())
                {
                    if (first) { bounds = r.bounds; first = false; } else bounds.Encapsulate(r.bounds);
                    string names = string.Empty;
                    foreach (Material material in r.sharedMaterials) names += material.name + ",";
                    Debug.Log("HOME_MODEL_RENDERER " + id + " " + r.name + " " + names);
                }
                Debug.Log("HOME_MODEL_BOUNDS " + id + " " + bounds.size.ToString("F3") + " center=" + bounds.center.ToString("F3"));
                Object.DestroyImmediate(instance);
            }
        }
    }
}
