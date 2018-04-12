using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class UGLFBundleBuilder
{
    public static void BuildBundle(BuildTarget _targetPlatform, string _abResPath, string _bundleTargetPath)
    {
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
        BuildPipeline.BuildAssetBundles(_bundleTargetPath, builds.ToArray(), option, _targetPlatform);

        // rename
        
        //File.Move()

    }

    private static string GetBundleSummaryFileName(BuildTarget _targetPlatform)
    {
        switch (_targetPlatform)
        {
            case BuildTarget.StandaloneWindows64:
                {

                }
                break;
        }

        return "";
    }
}
