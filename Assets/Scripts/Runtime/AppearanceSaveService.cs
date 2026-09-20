using System;
using System.IO;
using LightweightGame.Core;
using UnityEngine;

namespace LightweightGame.Runtime
{
    public static class AppearanceSaveService
    {
        internal static string TestPath;
        public static string SavePath
        {
            get { return TestPath ?? Path.Combine(Application.persistentDataPath, "appearance.json"); }
        }

        public static AppearanceData LoadOrDefault()
        {
            try
            {
                if (!File.Exists(SavePath))
                    return AppearanceData.CreateDefault();

                AppearanceData data = JsonUtility.FromJson<AppearanceData>(File.ReadAllText(SavePath));
                if (data == null)
                    return AppearanceData.CreateDefault();

                data.UpgradeLegacy();
                data.Normalize();
                return data;
            }
            catch (Exception error)
            {
                Debug.LogWarning("Could not load appearance data: " + error.Message);
                return AppearanceData.CreateDefault();
            }
        }

        public static bool Save(AppearanceData data)
        {
            if (data == null)
                return false;

            try
            {
                data.Normalize();
                File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
                return true;
            }
            catch (Exception error)
            {
                Debug.LogError("Could not save appearance data: " + error.Message);
                return false;
            }
        }
    }
}
