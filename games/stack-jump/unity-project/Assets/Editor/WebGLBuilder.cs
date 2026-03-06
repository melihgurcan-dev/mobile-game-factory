#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class WebGLBuilder
{
    [MenuItem("Stack Jump/Build WebGL")]
    public static void BuildFromMenu()
    {
        bool ok = RunBuild();
        if (ok)
            EditorUtility.DisplayDialog("WebGL Build Succeeded",
                "Build complete!\n\nPath: Builds/WebGL/\n\nZip that folder and upload to itch.io.", "OK");
        else
            EditorUtility.DisplayDialog("WebGL Build Failed",
                "Build failed — check the Console for errors.", "OK");
    }

    private static bool RunBuild()
    {
        string outputDir = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "..", "Builds", "WebGL"));

        Directory.CreateDirectory(outputDir);

        PlayerSettings.companyName   = "MobileGameFactory";
        PlayerSettings.productName   = "Stack Jump";
        PlayerSettings.bundleVersion = "0.1.0";

        // WebGL specific
        PlayerSettings.WebGL.compressionFormat     = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.template              = "APPLICATION:Default";

        var options = new BuildPlayerOptions
        {
            scenes           = new[] { "Assets/Scenes/Game.unity" },
            locationPathName = outputDir,
            target           = BuildTarget.WebGL,
            options          = BuildOptions.None
        };

        Debug.Log($"[WebGLBuilder] Building to: {outputDir}");

        BuildReport  report  = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[WebGLBuilder] SUCCESS — {summary.totalSize / 1048576} MB  →  {outputDir}");
            return true;
        }
        else
        {
            Debug.LogError($"[WebGLBuilder] FAILED: {summary.result}  errors:{summary.totalErrors}");
            return false;
        }
    }
}
#endif
