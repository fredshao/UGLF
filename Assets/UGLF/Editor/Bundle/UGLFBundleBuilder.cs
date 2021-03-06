﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class UGLFBundleBuilder {

    private const string BundleInfoFile = "bundles.info";

    /// <summary>
    /// 打包所有的 Bundle
    /// </summary>
    /// <param name="_targetPlatform"></param>
    /// <param name="_abResPath"></param>
    /// <param name="_bundleTargetPath"></param>
    /// <param name="_autoDelManifestFile"></param>
    /// <returns></returns>
    public static Dictionary<string, Hash128> BuildBundles(BuildTarget _targetPlatform, string _abResPath, string _bundleTargetPath, bool _autoDelManifestFile = true) {

        if (!Directory.Exists(_abResPath)) {
            Debug.LogError("请选择一个有效的目录!");
            return null;
        }

        string directoryName = GetBundleSummaryFileName(_targetPlatform);
        string realBundleTargetPath = Path.Combine(_bundleTargetPath, directoryName);
        MakeDirectoryClean(realBundleTargetPath);

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

        string[] directories = Directory.GetDirectories(_abResPath, "*.*", SearchOption.AllDirectories);

        if (directories == null || directories.Length == 0) {
            Debug.LogError("当前目录下没有子目录，请尝试使用 BuildSingleBundle!");
            return null;
        }

        foreach (string directory in directories) {
            string bundleName = Path.GetFileNameWithoutExtension(directory);
            string[] fileArray = GetAvaliableBundleAssetFile(directory);

            if (fileArray.Length > 0) {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = bundleName + ".ab";
                build.assetNames = fileArray;
                builds.Add(build);
            }
        }

        if (builds.Count == 0) {
            Debug.LogError("未发现需要打包的资源");
            return null;
        }

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(realBundleTargetPath, builds.ToArray(), option, _targetPlatform);

        if (_autoDelManifestFile) {
            DeleteExtManifestFile(realBundleTargetPath);
        }

        string manifestBundleFile = new DirectoryInfo(realBundleTargetPath).Name;
        File.Delete(Path.Combine(realBundleTargetPath, manifestBundleFile));


        Dictionary<string, Hash128> bundleHashDict = GetAllBundleInfoFromManifest(manifest);
        GenerateBundleInfo(realBundleTargetPath, bundleHashDict);
        Debug.Log("Bundle打包完成!");
        return bundleHashDict;
    }

    /// <summary>
    /// 打包一个目录及子目录的 Bundle
    /// </summary>
    /// <param name="_targetPlatform"></param>
    /// <param name="_abResPath"></param>
    /// <param name="_bundleTargetPath"></param>
    /// <param name="_autoDelManifestFile"></param>
    /// <returns></returns>
    public static Dictionary<string, Hash128> BuildMultipleBundles(BuildTarget _targetPlatform, string _abResPath, string _bundleTargetPath, bool _autoDelManifestFile = true) {

        if (!Directory.Exists(_abResPath)) {
            Debug.LogError("请选择一个有效的目录!");
            return null;
        }

        string directoryName = GetBundleSummaryFileName(_targetPlatform);

        _bundleTargetPath = Path.Combine(_bundleTargetPath, directoryName);

        MakeSureDirectoryExists(_bundleTargetPath);

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();

        string[] directories = Directory.GetDirectories(_abResPath, "*.*", SearchOption.AllDirectories);

        if (directories == null || directories.Length == 0) {
            Debug.LogError("当前目录下没有子目录，请尝试使用 BuildSingleBundle!");
            return null;
        }

        foreach (string directory in directories) {
            string bundleName = Path.GetFileNameWithoutExtension(directory);
            string[] fileArray = GetAvaliableBundleAssetFile(directory);
            if (fileArray.Length > 0) {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = bundleName + ".ab";
                build.assetNames = fileArray;
                builds.Add(build);
            }
        }

        if (builds.Count == 0) {
            Debug.LogError("未发现需要打包的资源");
            return null;
        }

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(_bundleTargetPath, builds.ToArray(), option, _targetPlatform);

        if (_autoDelManifestFile) {
            DeleteExtManifestFile(_bundleTargetPath);
        }

        string manifestBundleFile = new DirectoryInfo(_bundleTargetPath).Name;
        File.Delete(Path.Combine(_bundleTargetPath, manifestBundleFile));

        Dictionary<string, Hash128> bundleHashDict = GetAllBundleInfoFromManifest(manifest);
        Dictionary<string, BundleInfo> bundleInfoDict = LoadOriginBundleInfo(_bundleTargetPath);
        Dictionary<string, ulong> fileSizeDict = GetFileSizeDict(_bundleTargetPath);

        var enumer = bundleHashDict.GetEnumerator();
        while (enumer.MoveNext())
        {
            string bundleName = enumer.Current.Key;
            Hash128 bundleHash = enumer.Current.Value;
            if (bundleInfoDict.ContainsKey(bundleName))
            {
                bundleInfoDict[bundleName].bundleSize = fileSizeDict[bundleName];
                bundleInfoDict[bundleName].bundleHash = bundleHash;
            }
            else
            {
                BundleInfo info = new BundleInfo(bundleName, fileSizeDict[bundleName], bundleHash);
                bundleInfoDict.Add(info.bundleName, info);
            }
        }

        GenerateBundleInfo(_bundleTargetPath, bundleInfoDict);

        Debug.Log("Bundle打包完成!");

        return bundleHashDict;
    }

    /// <summary>
    /// 打包单个目录的 Bundle
    /// </summary>
    /// <param name="_targetPlatform"></param>
    /// <param name="_abSingleResPath"></param>
    /// <param name="_bundleTargetPath"></param>
    /// <param name="_autoDelManifestFile"></param>
    /// <returns></returns>
    public static Dictionary<string, Hash128> BuildSingleBundle(BuildTarget _targetPlatform, string _abSingleResPath, string _bundleTargetPath, bool _autoDelManifestFile = true) {

        if (!Directory.Exists(_abSingleResPath)) {
            Debug.LogError("请选择一个有效的目录!");
            return null;
        }

        string[] files = GetAvaliableBundleAssetFile(_abSingleResPath);

        if (files == null || files.Length == 0) {
            Debug.LogError("未发现需要打包的资源");
            return null;
        }

        string directoryName = GetBundleSummaryFileName(_targetPlatform);

        _bundleTargetPath = Path.Combine(_bundleTargetPath, directoryName);

        MakeSureDirectoryExists(_bundleTargetPath);


        string bundleName = Path.GetFileNameWithoutExtension(_abSingleResPath);
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = new DirectoryInfo(_abSingleResPath).Name + ".ab";
        build.assetNames = files;

        AssetBundleBuild[] builds = { build };

        BuildAssetBundleOptions option = BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(_bundleTargetPath, builds, option, _targetPlatform);

        if (_autoDelManifestFile) {
            DeleteExtManifestFile(_bundleTargetPath);
        }

        string manifestBundleFile = new DirectoryInfo(_bundleTargetPath).Name;
        File.Delete(Path.Combine(_bundleTargetPath, manifestBundleFile));


        Dictionary<string, Hash128> bundleHashDict = GetAllBundleInfoFromManifest(manifest);
        Dictionary<string, BundleInfo> bundleInfoDict = LoadOriginBundleInfo(_bundleTargetPath);
        Dictionary<string, ulong> fileSizeDict = GetFileSizeDict(_bundleTargetPath);

        var enumer = bundleHashDict.GetEnumerator();
        while (enumer.MoveNext())
        {
            bundleName = enumer.Current.Key;
            Hash128 bundleHash = enumer.Current.Value;
            if (bundleInfoDict.ContainsKey(bundleName))
            {
                bundleInfoDict[bundleName].bundleSize = fileSizeDict[bundleName];
                bundleInfoDict[bundleName].bundleHash = bundleHash;
            }
            else
            {
                BundleInfo info = new BundleInfo(bundleName, fileSizeDict[bundleName], bundleHash);
                bundleInfoDict.Add(info.bundleName, info);
            }
        }

        GenerateBundleInfo(_bundleTargetPath, bundleInfoDict);

        Debug.Log("Bundle打包完成!");
        return bundleHashDict;

    }

    private static Dictionary<string, ulong> GetFileSizeDict(string _path)
    {
        Dictionary<string, ulong> fileSizeDict = new Dictionary<string, ulong>();
        FileInfo[] files = new DirectoryInfo(_path).GetFiles();
        for(int i = 0; i < files.Length; ++i)
        {
            FileInfo info = files[i];
            fileSizeDict.Add(info.Name, (ulong)info.Length);
        }

        return fileSizeDict;
    }


    private static void GenerateBundleInfo(string _path, Dictionary<string, Hash128> _bundleHashDict) {
        FileInfo[] files = new DirectoryInfo(_path).GetFiles();
        string bundleInfoFile = Path.Combine(_path, BundleInfoFile);
        string infoStr = "";
        for (int i = 0; i < files.Length; ++i) {
            FileInfo info = files[i];

            if (Path.GetExtension(info.Name) != ".ab") {
                continue;
            }

            infoStr += string.Format("{0},{1},{2}", info.Name, info.Length, _bundleHashDict[info.Name].ToString());

            if (i + 1 != files.Length) {
                infoStr += '\n';
            }
        }

        File.WriteAllText(bundleInfoFile, infoStr);
    }

    private static void GenerateBundleInfo(string _path, Dictionary<string, BundleInfo> _bundleInfoDict)
    {
        var enumer = _bundleInfoDict.GetEnumerator();
        string infoStr = "";
        int index = 0;
        while (enumer.MoveNext())
        {
            BundleInfo info = enumer.Current.Value;
            infoStr += string.Format("{0},{1},{2}", info.bundleName, info.bundleSize, info.bundleHash.ToString());

            ++index;
            if(index != _bundleInfoDict.Count)
            {
                infoStr += '\n';
            }
        }

        string bundleInfoFile = Path.Combine(_path, BundleInfoFile);
        File.WriteAllText(bundleInfoFile, infoStr);
    }


    /// <summary>
    /// 获取一个目录下的所有非.meta文件，用于打包 Bundle
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
    private static string[] GetAvaliableBundleAssetFile(string _path) {
        List<string> fileList = new List<string>();
        string[] fileArray = Directory.GetFiles(_path, "*.*", SearchOption.TopDirectoryOnly);
        foreach (string file in fileArray) {
            if (Path.GetExtension(file) == ".meta") {
                continue;
            }
            fileList.Add(file);
        }

        return fileList.ToArray();
    }


    /// <summary>
    /// 把一个目录先删除再创建，确保干净
    /// </summary>
    /// <param name="_platformDirectoy"></param>
    private static void MakeDirectoryClean(string _platformDirectoy) {
        if (Directory.Exists(_platformDirectoy)) {
            Directory.Delete(_platformDirectoy, true);
        }

        Directory.CreateDirectory(_platformDirectoy);
    }


    /// <summary>
    /// 如果一个目录不存在，则创建
    /// </summary>
    /// <param name="_path"></param>
    private static void MakeSureDirectoryExists(string _path) {
        if (Directory.Exists(_path) == false) {
            Directory.CreateDirectory(_path);
        }
    }


    /// <summary>
    /// 删除扩展名为 manifest 的文件
    /// </summary>
    /// <param name="_path"></param>
    private static void DeleteExtManifestFile(string _path) {
        foreach (string file in Directory.GetFiles(_path)) {
            if (Path.GetExtension(file) == ".manifest") {
                File.Delete(file);
            }
        }
    }


    /// <summary>
    /// 从 Manifest 中获取所有的 Bundle 名字和 Bundle Hash128
    /// </summary>
    /// <param name="_manifest"></param>
    /// <returns></returns>
    private static Dictionary<string, Hash128> GetAllBundleInfoFromManifest(AssetBundleManifest _manifest) {
        Dictionary<string, Hash128> bundleInfoDict = new Dictionary<string, Hash128>();
        string[] allBundleNames = _manifest.GetAllAssetBundles();
        for (int i = 0; i < allBundleNames.Length; ++i) {
            string bundleName = allBundleNames[i];
            Hash128 bundleHash = _manifest.GetAssetBundleHash(bundleName);
            bundleInfoDict.Add(bundleName, bundleHash);
        }

        return bundleInfoDict;
    }


    /// <summary>
    /// 获取对应平台Bundle应该放在的目录名
    /// </summary>
    /// <param name="_targetPlatform"></param>
    /// <returns></returns>
    private static string GetBundleSummaryFileName(BuildTarget _targetPlatform) {
        switch (_targetPlatform) {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneWindows: {
                    return "Windows";
                }
            case BuildTarget.Android: {
                    return "Android";
                }
            case BuildTarget.iOS: {
                    return "iOS";
                }
            case BuildTarget.StandaloneOSX: {
                    return "OSX";
                }
            default: {
                    return "Bundles";
                }
        }
    }


    private static Dictionary<string, BundleInfo> LoadOriginBundleInfo(string _targetPath)
    {
        Dictionary<string, BundleInfo> bundleInfoDict = new Dictionary<string, BundleInfo>();
        string path = Path.Combine(_targetPath, BundleInfoFile);
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            string[] bundleLines = text.Split('\n');
            
            foreach(string bundleInfoStr in bundleLines)
            {
                string[] bundleInfoArray = bundleInfoStr.Split(',');
                string bundleName = bundleInfoArray[0];
                ulong bundleSize = ulong.Parse(bundleInfoArray[1]);
                Hash128 bundleHash = Hash128.Parse(bundleInfoArray[2]);
                BundleInfo info = new BundleInfo(bundleName, bundleSize, bundleHash);
                bundleInfoDict.Add(info.bundleName, info);
            }

            
        }

        return bundleInfoDict;
    }


    internal class BundleInfo
    {
        public string bundleName;
        public ulong bundleSize;
        public Hash128 bundleHash;

        public BundleInfo(string _bundleName, ulong _bundleSize, Hash128 _bundleHash)
        {
            bundleName = _bundleName;
            bundleSize = _bundleSize;
            bundleHash = _bundleHash;
        }
    }
}
