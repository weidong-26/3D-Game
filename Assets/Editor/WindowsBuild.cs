using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LightweightGame.Editor
{
    public static class WindowsBuild
    {
        private const string MainScene = "Assets/Scenes/Main.unity";
        private const string OutputPath = "Builds/Windows/3DGamePrototype.exe";

        public static void Build()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            string scenePath = Path.Combine(projectRoot, MainScene);
            if (!File.Exists(scenePath))
                throw new FileNotFoundException("Startup scene is missing.", scenePath);

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainScene, true)
            };

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { MainScene },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.StrictMode
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception(
                    "Windows build failed with " + report.summary.totalErrors +
                    " error(s) and " + report.summary.totalWarnings + " warning(s)."
                );
            }

            Debug.Log(
                "Windows build completed: " + report.summary.outputPath +
                " (" + report.summary.totalSize + " bytes)"
            );
        }
    }
}
