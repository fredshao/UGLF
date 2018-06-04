using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public enum En_UGLFBundleState
{
    None,                                   // 未开始 Bundle 的更新
    UpdateRemoteBundle,                     // 正在更新远程 Bundle
    LoadLocalBundle,                        // 正在加载本地缓存 Bundle
    AllBundleUpdateSuccessfulAndLoaded,     // 所有的 Bundle 已更新完毕，并且加载成功
    UpdateRemoteBundleFaild,                // 远程 Bundle 更新失败
    LoadLocalBundleFaild,                   // 本地缓存 Bundle 加载失败
}

public class UGLFBundle {

    enum Internal_En_BundleState
    {
        None,
        DownloadingBundleInfoFile,
        DownloadingRemoteBundle,
        LoadingLocalBundle,
        UpdateSuccessful,
        DownloadBundleInfoFileError,
        DownloadRemoteBundleError,
        LoadLocalBundleError,
    }


    private string remoteUrl = string.Empty;
    private const string BundleInfoFile = "bundles.info";

    private Dictionary<string, BundleInfo> unCachedBundleDict = null;
    private Dictionary<string, BundleInfo> cachedBundleDict = null;

    private Dictionary<string, AssetBundle> bundleName2BundleDict = new Dictionary<string, AssetBundle>();
    private Dictionary<string, AssetBundle> assetPath2BundleDict = new Dictionary<string, AssetBundle>();
    private Dictionary<string, UnityEngine.Object> assetPath2AssetDict = new Dictionary<string, UnityEngine.Object>();

    private Internal_En_BundleState _internalBundleState = Internal_En_BundleState.None;
    private Internal_En_BundleState internalBundleState {
        get {
            return _internalBundleState;
        }
        set {
            _internalBundleState = value;
            switch (_internalBundleState)
            {
                case Internal_En_BundleState.DownloadingBundleInfoFile:
                case Internal_En_BundleState.DownloadingRemoteBundle:
                    {
                        bundleState = En_UGLFBundleState.UpdateRemoteBundle;
                    }
                    break;
                case Internal_En_BundleState.LoadingLocalBundle:
                    {
                        bundleState = En_UGLFBundleState.LoadLocalBundle;
                    }
                    break;
                case Internal_En_BundleState.DownloadBundleInfoFileError:
                case Internal_En_BundleState.DownloadRemoteBundleError:{
                        bundleState = En_UGLFBundleState.UpdateRemoteBundleFaild;
                    }
                    break;
                case Internal_En_BundleState.LoadLocalBundleError:
                    {
                        bundleState = En_UGLFBundleState.LoadLocalBundleFaild;
                    }
                    break;
                case Internal_En_BundleState.UpdateSuccessful:
                    {
                        bundleState = En_UGLFBundleState.AllBundleUpdateSuccessfulAndLoaded;
                    }
                    break;
            }
        }
    }


    private En_UGLFBundleState _bundleState = En_UGLFBundleState.None;

    /// <summary>
    /// 对外状态
    /// </summary>
    public En_UGLFBundleState bundleState {
        get { return _bundleState; }
        set {
            _bundleState = value;
        }
    }

    /// <summary>
    /// 一共要下载多少字节
    /// </summary>
    public ulong totalNeedDownloadBytes = 0;
    /// <summary>
    /// 已经下载完多少字节
    /// </summary>
    public ulong downloadedBytes = 0;
    /// <summary>
    /// 当前的Web下载请求
    /// </summary>
    private UnityWebRequest currWebRequest = null;

    /// <summary>
    /// 当前的更新进度
    /// </summary>
    public int Progress {
        get {
            ulong currDownloadedBytes = 0;
            if(currWebRequest != null)
            {
                currDownloadedBytes = currWebRequest.downloadedBytes;
            }

            if (bundleState == En_UGLFBundleState.AllBundleUpdateSuccessfulAndLoaded)
            {
                return 100;
            }

            if (totalNeedDownloadBytes == 0)
            {
                return 0;
            }

            return (int)((float)(currDownloadedBytes + downloadedBytes) / (float)totalNeedDownloadBytes * 100.0f);
        }
    }


    public UGLFBundle(string _remoteUrl)
    {
        remoteUrl = _remoteUrl;
    }

    /// <summary>
    /// 开始更新
    /// </summary>
    public void StartUpdate()
    {
        string bundleInfoUrl = _CombineRemotePath(BundleInfoFile);
        UGLFBundleLoader loader = new UGLFBundleLoader(bundleInfoUrl, string.Empty , _Internal_OnBundleInfoCompleted, EN_RequestType.Text);
        internalBundleState = Internal_En_BundleState.DownloadingBundleInfoFile;
        loader.StartDownload();
    }

    /// <summary>
    /// 获取一个资源
    /// </summary>
    /// <param name="_fullAssetPath"></param>
    /// <returns></returns>
    public UnityEngine.Object GetAsset(string _fullAssetPath)
    {
        string assetPath = _fullAssetPath.ToLower();

        if (assetPath2AssetDict.ContainsKey(assetPath))
        {
            return assetPath2AssetDict[assetPath];
        }

        AssetBundle bundle = null;
        assetPath2BundleDict.TryGetValue(assetPath, out bundle);
        if(bundle == null)
        {
            Debug.LogError("获取资源所在的Bundle失败！" + _fullAssetPath);
            return null;
        }

        UnityEngine.Object rawAsset = bundle.LoadAsset(assetPath);
        if (rawAsset != null)
        {
            assetPath2AssetDict.Add(assetPath, rawAsset);
        }
        else
        {
            Debug.LogError("资源加载失败：" + _fullAssetPath);
        }
        return rawAsset;
    }

    /// <summary>
    /// 获取一个指定类型的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_fullAssetPath"></param>
    /// <returns></returns>
    public T GetAsset<T>(string _fullAssetPath) where T : UnityEngine.Object
    {
        string assetPath = _fullAssetPath.ToLower();

        if (assetPath2AssetDict.ContainsKey(assetPath))
        {
            return assetPath2AssetDict[assetPath] as T;
        }

        AssetBundle bundle = null;
        assetPath2BundleDict.TryGetValue(assetPath, out bundle);
        if(bundle == null)
        {
            Debug.LogError("获取资源所在的Bundle失败！" + _fullAssetPath);
            return null;
        }

        T rawAsset = bundle.LoadAsset<T>(assetPath);
        if(rawAsset != null)
        {
            assetPath2AssetDict.Add(assetPath, rawAsset);
        }
        else
        {
            Debug.LogError("资源加载失败：" + _fullAssetPath);
        }

        return rawAsset;
    }

    /// <summary>
    /// 获取一个克隆GameObject
    /// </summary>
    /// <param name="_fullAssetPath"></param>
    /// <returns></returns>
    public GameObject GetCloneGameObject(string _fullAssetPath)
    {
        GameObject rawObj = GetAsset<GameObject>(_fullAssetPath);
        if(rawObj == null)
        {
            return null;
        }
        return GameObject.Instantiate(rawObj);
    }

    /// <summary>
    /// 卸载一个资源
    /// </summary>
    /// <param name="_fullAssetPath"></param>
    /// <returns></returns>
    public bool UnloadAsset(string _fullAssetPath)
    {
        string assetPath = _fullAssetPath.ToLower();
        UnityEngine.Object rawAsset = null;
        assetPath2AssetDict.TryGetValue(assetPath, out rawAsset);
        if(rawAsset != null)
        {
            assetPath2AssetDict.Remove(assetPath);
            Resources.UnloadAsset(rawAsset);
            return true;
        }
        return false;
    }


    /// <summary>
    /// 内部流程，加载 Bundle 信息文件
    /// </summary>
    /// <param name="_request"></param>
    private void _Internal_OnBundleInfoCompleted(UnityWebRequest _request)
    {
        if (_request == null)
        {
            internalBundleState = Internal_En_BundleState.DownloadBundleInfoFileError;
            return;
        }

        string infoText = _request.downloadHandler.text;
        if (string.IsNullOrEmpty(infoText))
        {
            internalBundleState = Internal_En_BundleState.DownloadBundleInfoFileError;
            return;
        }

        // 解析远程所有的 Bundle 信息
        List<BundleInfo> bundleInfoList = new List<BundleInfo>();
        string[] bundleinfoStrArray = infoText.Split('\n');
        foreach (string bundleInfoStr in bundleinfoStrArray)
        {
            string[] bundleInfoArray = bundleInfoStr.Split(',');
            string bundleName = bundleInfoArray[0];
            ulong length = ulong.Parse(bundleInfoArray[1]);
            Hash128 bundleHash = Hash128.Parse(bundleInfoArray[2]);
            string bundleUrl = _CombineRemotePath(bundleName);
            BundleInfo bundleInfo = new BundleInfo(bundleUrl, bundleName, length, bundleHash);
            bundleInfoList.Add(bundleInfo);
        }

        // 判断哪一些 Bundle 未下载

        // 初始化要下载的数据大小
        totalNeedDownloadBytes = 0;
        downloadedBytes = 0;

        unCachedBundleDict = new Dictionary<string, BundleInfo>();
        cachedBundleDict = new Dictionary<string, BundleInfo>();

        for (int i = 0; i < bundleInfoList.Count; ++i)
        {
            BundleInfo info = bundleInfoList[i];
            bool isCached = Caching.IsVersionCached(info.bundleUrl, info.bundleHash);
            if (isCached)
            {
                cachedBundleDict.Add(info.bundleUrl, info);
            }
            else
            {
                unCachedBundleDict.Add(info.bundleUrl, info);
                totalNeedDownloadBytes += info.bundleSize;
            }
        }

        _Internal_DownloadRemoteBundle(unCachedBundleDict.GetEnumerator());
    }


    /// <summary>
    /// 内部流程，开始下载远程 Bundle
    /// </summary>
    /// <param name="_enumer"></param>
    private void _Internal_DownloadRemoteBundle(Dictionary<string, BundleInfo>.Enumerator _enumer)
    {
        if (_enumer.MoveNext())
        {
            BundleInfo info = _enumer.Current.Value;
            UGLFBundleLoader loader = new UGLFBundleLoader(info.bundleUrl, info.bundleHash.ToString(), (UnityWebRequest _request)=>
            {
                currWebRequest = null;
                if (_request.isNetworkError)
                {
                    internalBundleState = Internal_En_BundleState.DownloadRemoteBundleError;
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(_request);
                    bundleName2BundleDict.Add(bundle.name, bundle);
                    string[] assetNames = bundle.GetAllAssetNames();
                    foreach (string assetFullName in assetNames)
                    {
                        if (assetPath2BundleDict.ContainsKey(assetFullName) == false)
                        {
                            assetPath2BundleDict.Add(assetFullName, bundle);
                        }
                    }

                    downloadedBytes += info.bundleSize;
                    // 递归下载 远程 Bundle
                    _Internal_DownloadRemoteBundle(_enumer);
                }
            }, EN_RequestType.AssetBundle);

            internalBundleState = Internal_En_BundleState.DownloadingRemoteBundle;
            loader.StartDownload();
            currWebRequest = loader.request;
        }
        else
        {
            // 开始下载本地缓存好的 Bundle
            _Internal_LoadLocalBundle(cachedBundleDict.GetEnumerator());
        }
    }

    /// <summary>
    /// 内部流程，加载本地缓存的 Bundle
    /// </summary>
    /// <param name="_enumer"></param>
    private void _Internal_LoadLocalBundle(Dictionary<string, BundleInfo>.Enumerator _enumer)
    {
        if (_enumer.MoveNext())
        {
            BundleInfo info = _enumer.Current.Value;
            UGLFBundleLoader loader = new UGLFBundleLoader(info.bundleUrl, info.bundleHash.ToString(), (UnityWebRequest _request) =>
            {
                if (_request.isNetworkError)
                {
                    internalBundleState = Internal_En_BundleState.LoadLocalBundleError;
                }
                else
                {
                    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(_request);
                    bundleName2BundleDict.Add(bundle.name, bundle);
                    string[] assetNames = bundle.GetAllAssetNames();
                    foreach (string assetFullName in assetNames)
                    {
                        if (assetPath2BundleDict.ContainsKey(assetFullName) == false)
                        {
                            assetPath2BundleDict.Add(assetFullName, bundle);
                        }
                    }

                    // 递归下载 远程 Bundle
                    _Internal_LoadLocalBundle(_enumer);
                }
            }, EN_RequestType.AssetBundle);
            internalBundleState = Internal_En_BundleState.LoadingLocalBundle;
            loader.StartDownload();
        }
        else
        {
            // Bundle 更新流程完成
            internalBundleState = Internal_En_BundleState.UpdateSuccessful;
            
        }
    }

    /// <summary>
    /// 构建远程Bundle路径
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    private string _CombineRemotePath(string _name)
    {
        int len = remoteUrl.Length;
        if(remoteUrl[len - 1] != '/')
        {
            return remoteUrl + "/" + _GetBundleRootFolder() + "/" + _name;
        }
        else
        {
            return remoteUrl + _GetBundleRootFolder() + "/" + _name;
        }
    }

    /// <summary>
    /// 获取运行平台对应的Bundle目录
    /// </summary>
    /// <returns></returns>
    private static string _GetBundleRootFolder()
    {
        RuntimePlatform platform = Application.platform;
        switch (platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                {
                    return "Windows";
                }
            case RuntimePlatform.Android:
                {
                    return "Android";
                }
            case RuntimePlatform.IPhonePlayer:
                {
                    return "iOS";
                }
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                {
                    return "OSX";
                }
            default:
                {
                    return "Bundles";
                }
        }
    }



    internal class BundleInfo
    {
        public string bundleUrl;
        public string bundleName;
        public ulong bundleSize;
        public Hash128 bundleHash;

        public BundleInfo(string _bundleUrl, string _bundleName, ulong _bundleSize, Hash128 _bundleHash)
        {
            bundleUrl = _bundleUrl;
            bundleName = _bundleName;
            bundleSize = _bundleSize;
            bundleHash = _bundleHash;
        }
    }

    internal enum EN_RequestType
    {
        Text,
        AssetBundle,
        Texture,
        AudioClip,
    }

    internal class UGLFBundleLoader
    {
        private string url = string.Empty;
        private string hashStr = string.Empty;
        private AsyncOperation operation;
        private Action<UnityWebRequest> onCompleteCallback;
        public UnityWebRequest request;
        private EN_RequestType requestType;

        public int progress {
            get {
                if(operation == null)
                {
                    return 0;
                }
                else
                {
                    return (int)(operation.progress * 100);
                }
            }
        }

    
        public UGLFBundleLoader(string _url, string _hashStr , Action<UnityWebRequest> _completeCallback, EN_RequestType _requestType)
        {
            url = _url;
            hashStr = _hashStr;
            requestType = _requestType;
            onCompleteCallback = _completeCallback;
        }

        public void StartDownload()
        {
            switch (requestType)
            {
                case EN_RequestType.Text:
                    {
                        request = UnityWebRequest.Get(url);
                    }
                    break;
                case EN_RequestType.AssetBundle:
                    {
                        if (!string.IsNullOrEmpty(hashStr))
                        {
                            request = UnityWebRequest.GetAssetBundle(url, Hash128.Parse(hashStr), 0);
                        }
                        else
                        {
                            request = UnityWebRequest.GetAssetBundle(url);
                        }
                    }
                    break;
                case EN_RequestType.Texture:
                    {
                        request = UnityWebRequestTexture.GetTexture(url);
                    }
                    break;
                case EN_RequestType.AudioClip:
                    {
                        request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS);
                    }
                    break;
            }
            
            operation = request.SendWebRequest();
            operation.completed += OnComplete;
        }

        private void OnComplete(AsyncOperation _op)
        {
            if (request.isNetworkError)
            {
                onCompleteCallback(null);
            }
            else
            {
                onCompleteCallback(request);
            }
        }
    }


}