namespace UnityEditor
{
    public sealed class EditorBuildSettingsScene
    {
        public EditorBuildSettingsScene(string path, bool enabled) { }
    }

    public static class EditorBuildSettings
    {
        public static EditorBuildSettingsScene[] scenes;
    }

    public struct BuildPlayerOptions
    {
        public string[] scenes;
        public string locationPathName;
        public BuildTarget target;
        public BuildOptions options;
    }

    public enum BuildTarget
    {
        StandaloneWindows64
    }

    public enum BuildOptions
    {
        StrictMode
    }

    public static class BuildPipeline
    {
        public static Build.Reporting.BuildReport BuildPlayer(BuildPlayerOptions options)
        {
            return new Build.Reporting.BuildReport();
        }
    }
}

namespace UnityEditor.Build.Reporting
{
    public sealed class BuildReport
    {
        public BuildSummary summary;
    }

    public struct BuildSummary
    {
        public BuildResult result;
        public int totalErrors;
        public int totalWarnings;
        public string outputPath;
        public ulong totalSize;
    }

    public enum BuildResult
    {
        Succeeded,
        Failed
    }
}
