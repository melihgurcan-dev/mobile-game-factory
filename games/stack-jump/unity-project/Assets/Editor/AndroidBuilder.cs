/// <summary>
/// Android APK build script.
/// • Editor menu:  Stack Jump → Build Android APK
/// • Batchmode:    Unity.exe -batchmode -executeMethod AndroidBuilder.BuildBatchmode ...
///                 (requires Unity Pro headless entitlement)
/// </summary>
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public static class AndroidBuilder
{
    // ── Called from Editor menu ────────────────────────────────────────
    [MenuItem("Stack Jump/Build Android APK")]
    public static void BuildFromMenu()
    {
        bool ok = RunBuild(development: false);
        if (ok)
            EditorUtility.DisplayDialog("Build Succeeded",
                "APK built successfully!\n\nPath: Builds/Android/StackJump.apk", "OK");
        else
            EditorUtility.DisplayDialog("Build Failed",
                "Build failed — check the Console for errors.", "OK");
    }

    [MenuItem("Stack Jump/Build Android APK (Development)")]
    public static void BuildFromMenuDev()
    {
        RunBuild(development: true);
    }

    // ── Called via -batchmode -executeMethod AndroidBuilder.BuildBatchmode
    //    Requires Unity Pro / headless licence
    public static void BuildBatchmode()
    {
        bool ok = RunBuild(development: false);
        EditorApplication.Exit(ok ? 0 : 1);
    }

    // ── Core build logic ───────────────────────────────────────────────
    private static bool RunBuild(bool development)
    {
        string outputDir  = Path.GetFullPath(
            Path.Combine(Application.dataPath, "..", "..", "Builds", "Android"));
        string outputPath = Path.Combine(outputDir, "StackJump.apk");

        Directory.CreateDirectory(outputDir);

        // Player settings
        PlayerSettings.companyName   = "MobileGameFactory";
        PlayerSettings.productName   = "Stack Jump";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.SetApplicationIdentifier(
            BuildTargetGroup.Android, "com.mobilegamefactory.stackjump");

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.Android.minSdkVersion       = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.targetSdkVersion    = AndroidSdkVersions.AndroidApiLevel34;

        // Build options
        var buildOptions = development
            ? BuildOptions.Development | BuildOptions.AllowDebugging
            : BuildOptions.None;

        var options = new BuildPlayerOptions
        {
            scenes           = new[] { "Assets/Scenes/Game.unity" },
            locationPathName = outputPath,
            target           = BuildTarget.Android,
            options          = buildOptions
        };

        Debug.Log($"[AndroidBuilder] Building to: {outputPath}");

        BuildReport  report  = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[AndroidBuilder] SUCCESS — {summary.totalSize / 1048576} MB  →  {outputPath}");
            return true;
        }
        else
        {
            Debug.LogError($"[AndroidBuilder] FAILED: {summary.result}  " +
                           $"errors:{summary.totalErrors}  warnings:{summary.totalWarnings}");
            return false;
        }
    }
}
#endif
