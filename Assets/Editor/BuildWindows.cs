using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Luminia.Editor
{
    public static class BuildWindows
    {
        private const string OutputDirectory = "Build/Windows";
        private const string ExecutablePath = OutputDirectory + "/Luminia.exe";

        [MenuItem("Luminia/Build Windows")]
        public static void Build()
        {
            Directory.CreateDirectory(OutputDirectory);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Main.unity" },
                locationPathName = ExecutablePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.CleanBuildCache
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException(
                    $"Luminia Windows build failed: {report.summary.result}. See the Unity Console for details.");
            }

            Debug.Log($"Windows build completed: {Path.GetFullPath(ExecutablePath)}");
            EditorUtility.RevealInFinder(ExecutablePath);
        }

        public static void BuildFromCommandLine()
        {
            Build();
        }
    }
}
