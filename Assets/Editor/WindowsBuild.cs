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

        public static void BuildEnvironment()
        {
            const string home = "Assets/Scenes/Home_Environment_V2.unity";
            if (!File.Exists(Path.Combine(Directory.GetParent(Application.dataPath).FullName, home)))
                throw new FileNotFoundException("Home environment scene is missing.", home);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(home, true),
                new EditorBuildSettingsScene(MainScene, true)
            };
            BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { home, MainScene },
                locationPathName = "Builds/Windows/HomeEnvironmentV2.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.StrictMode
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception("Home environment Windows build failed: " + report.summary.totalErrors + " errors.");
            Debug.Log("HOME_WINDOWS_BUILD_PASS " + report.summary.outputPath + " bytes=" + report.summary.totalSize);
        }

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
