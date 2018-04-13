using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class UGLFBundleBuilder
{
    public static void BuildBundle(BuildTarget _targetPlatform, string _abResPath, string _bundleTargetPath, bool _autoDelManifestFile = true)
    {
        string logStr = "";
        string directoryName = GetBundleSummaryFileName(_targetPlatform);
        string realBundleTargetPath = Path.Combine(_bundleTargetPath, directoryName);
        MakeDirectoryClean(realBundleTargetPath);

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        foreach (string directory in Directory.GetDirectories(_abResPath, "*.*", SearchOption.AllDirectories))
        {
            string bundleName = Path.GetFileNameWithoutExtension(directory);
            string[] fileArray = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);
            List<string> fileList = new List<string>();
            Debug.Log(bundleName.ToLower() + ".ab");
            logStr += bundleName.ToLower() + ".ab\n";
            foreach(string file in fileArray)
            {
                if(Path.GetExtension(file) == ".meta")
                {
                    continue;
                }
                fileList.Add(file);
                Debug.Log("    -- " + file);
                logStr += "    -- " + file + "\n";
            }

            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName + ".ab";
            build.assetNames = fileList.ToArray();
            builds.Add(build);

            File.WriteAllText(Path.Combine(realBundleTargetPath, "log.txt"), logStr);
        }

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        BuildPipeline.BuildAssetBundles(realBundleTargetPath, builds.ToArray(), option, _targetPlatform);

        if (_autoDelManifestFile)
        {
            string[] files = Directory.GetFiles(realBundleTargetPath);
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".manifest")
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
