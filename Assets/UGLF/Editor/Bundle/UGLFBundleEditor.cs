using UnityEngine;
using UnityEditor;

public class UGLFBundleEditor
{
    [MenuItem("UGLF Tools/Bundle/清除本地缓存的 Bundle")]
    static void ClearLocalCachedBundle()
    {
        Caching.ClearCache();
        Debug.Log("本地缓存已清除!");
    }



    [MenuItem("Assets/打包 Windows Bundle/完整Bundle")]
    static void UGLFEditorBuildBundle_Windows_Whole()
    {
        UGLFEditorBuildBundle_Whole(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/打包 Windows Bundle/当前目录及子目录")]
    static void UGLFEditorBuildBundle_Windows_CurrentAndSub()
    {
        UGLFEditorBuildBundle_CurrentAndSub(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/打包 Windows Bundle/当前目录")]
    static void UGLFEditorBuildBundle_Windows_Current()
    {
        UGLFEditorBuildBundle_Current(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Assets/打包 OSX Bundle/完整Bundle")]
    static void UGLFEditorBuildBundle_OSX_Whole()
    {
        UGLFEditorBuildBundle_Whole(BuildTarget.StandaloneOSX);
    }

    [MenuItem("Assets/打包 OSX Bundle/当前目录及子目录")]
    static void UGLFEditorBuildBundle_OSX_CurrentAndSub()
    {
        UGLFEditorBuildBundle_CurrentAndSub(BuildTarget.StandaloneOSX);
    }

    [MenuItem("Assets/打包 OSX Bundle/当前目录")]
    static void UGLFEditorBuildBundle_OSX_Current()
    {
        UGLFEditorBuildBundle_Current(BuildTarget.StandaloneOSX);
    }

    [MenuItem("Assets/打包 iOS Bundle/完整Bundle")]
    static void UGLFEditorBuildBundle_iOS_Whole()
    {
        UGLFEditorBuildBundle_Whole(BuildTarget.iOS);
    }

    [MenuItem("Assets/打包 iOS Bundle/当前目录及子目录")]
    static void UGLFEditorBuildBundle_iOS_CurrentAndSub()
    {
        UGLFEditorBuildBundle_CurrentAndSub(BuildTarget.iOS);
    }

    [MenuItem("Assets/打包 iOS Bundle/当前目录")]
    static void UGLFEditorBuildBundle_iOS_Current()
    {
        UGLFEditorBuildBundle_Current(BuildTarget.iOS);
    }

    [MenuItem("Assets/打包 Android Bundle/完整Bundle")]
    static void UGLFEditorBuildBundle_Android_Whole()
    {
        UGLFEditorBuildBundle_Whole(BuildTarget.Android);
    }

    [MenuItem("Assets/打包 Android Bundle/当前目录及子目录")]
    static void UGLFEditorBuildBundle_Android_CurrentAndSub()
    {
        UGLFEditorBuildBundle_CurrentAndSub(BuildTarget.Android);
    }

    [MenuItem("Assets/打包 Android Bundle/当前目录")]
    static void UGLFEditorBuildBundle_Android_Current()
    {
        UGLFEditorBuildBundle_Current(BuildTarget.Android);
    }

    private static void UGLFEditorBuildBundle_Whole(BuildTarget _targetPlatform)
    {
        string targetBundlePath = Application.dataPath + "/../Bundles";
        string selectedPath = UGLFEditorGetSelectedPath();
        if (string.IsNullOrEmpty(selectedPath))
        {
            Debug.LogError("请选择正确的目录!");
            return;
        }
        UGLFBundleBuilder.BuildBundles(_targetPlatform, selectedPath, targetBundlePath, true);
    }

    private static void UGLFEditorBuildBundle_CurrentAndSub(BuildTarget _targetPlatform)
    {
        string targetBundlePath = Application.dataPath + "/../Bundles";
        string selectedPath = UGLFEditorGetSelectedPath();
        if (string.IsNullOrEmpty(selectedPath))
        {
            Debug.LogError("请选择正确的目录!");
            return;
        }
        UGLFBundleBuilder.BuildMultipleBundles(_targetPlatform, selectedPath, targetBundlePath, true);
    }

    private static void UGLFEditorBuildBundle_Current(BuildTarget _targetPlatform)
    {
        string targetBundlePath = Application.dataPath + "/../Bundles";
        string selectedPath = UGLFEditorGetSelectedPath();
        if (string.IsNullOrEmpty(selectedPath))
        {
            Debug.LogError("请选择正确的目录!");
            return;
        }
        UGLFBundleBuilder.BuildSingleBundle(_targetPlatform, selectedPath, targetBundlePath, true);
    }


    private static string UGLFEditorGetSelectedPath()
    {
        Object[] selected = Selection.GetFiltered<Object>(SelectionMode.Assets);

        if (selected == null || selected.Length == 0)
        {
            return string.Empty;
        }

        string path = AssetDatabase.GetAssetPath(selected[0]);

        return path;
    }


}