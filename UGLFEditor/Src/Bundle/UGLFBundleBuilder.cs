using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class UGLFBundleBuilder
{
    public static void BuildBundle(BuildTarget _targetPlatform, string _abResPath, string _bundleTargetPath, bool _autoDelManifestFile = true)
    {
        string directoryName = GetBundleSummaryFileName(_targetPlatform);
        string realBundleTargetPath = Path.Combine(_bundleTargetPath, directoryName);
        MakeDirectoryClean(realBundleTargetPath);

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        foreach (string directory in Directory.GetDirectories(_abResPath))
        {
            string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = Path.GetFileNameWithoutExtension(directory) + ".ab";
            build.assetNames = files;
            builds.Add(build);
        }

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        BuildPipeline.BuildAssetBundles(realBundleTargetPath, builds.ToArray(), option, _targetPlatform);

        if (_autoDelManifestFile)
        {
            string[] files = Directory.GetFiles(realBundleTargetPath);
            foreach(string file in files)
            {
                if(Path.GetExtension(file) == ".manifest")
                {
                    File.Delete(file);
                }
            }
        }
    }

    private static void MakeDirectoryClean(string _platformDirectoy)
    {
        if (Directory.Exists(_platformDirectoy))
        {
            Directory.Delete(_platformDirectoy,true);
        }

        Directory.CreateDirectory(_platformDirectoy);
    }


    private static string GetBundleSummaryFileName(BuildTarget _targetPlatform)
    {
        switch (_targetPlatform)
        {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneWindows:
                {
                    return "Windows";
                }
            case BuildTarget.Android:
                {
                    return "Android";
                }
            case BuildTarget.iOS:
                {
                    return "iOS";
                }
            case BuildTarget.StandaloneOSX:
                {
                    return "OSX";
                }
            default:
                {
                    return "Bundles";
                }
        }
    }
}
