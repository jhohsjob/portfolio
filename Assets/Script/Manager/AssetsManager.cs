using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class AssetLoadingTask
{
    private string _address;
    private Action<AssetLoadingTask> _loadingCompleteAction;
    private Action _loadingFailedAction;
    private bool _isValid;
    private AsyncOperationHandle _handle;
    private bool _isSingleRes;
    private bool _isLoadComplete;
    private Dictionary<string, UnityEngine.Object> _assetsDict;

    private AssetsManager _manager;

    public string address => _address;

    public bool isLoadComplete => _isLoadComplete;
    public bool isLoadSucceed => _handle.IsValid() && _handle.Status == AsyncOperationStatus.Succeeded;

    public AssetLoadingTask(string inAddress, AssetsManager inManager)
    {
        _address = inAddress;
        _isValid = true;
        _manager = inManager;
        _isLoadComplete = false;
    }

    public void LoadAsset<T>(Action<AssetLoadingTask> onLoadingComplete, Action onLoadingFailed = null) where T : UnityEngine.Object
    {
        if (_isLoadComplete == false)
        {
            _loadingCompleteAction = onLoadingComplete;
            _loadingFailedAction = onLoadingFailed;
            _handle = Addressables.LoadAssetAsync<T>(_address);
            _handle.Completed += OnLoadComplete;
            _isSingleRes = true;
        }
    }

    public void LoadAssets<T>(Action<AssetLoadingTask> onLoadingComplete) where T : UnityEngine.Object
    {
        if (_isLoadComplete == false)
        {
            _loadingCompleteAction = onLoadingComplete;
            _handle = Addressables.LoadAssetsAsync<T>(_address, null);
            _handle.Completed += OnLoadComplete;
            _isSingleRes = false;
        }
    }

    public void AppendLoadCompleteAction(Action<AssetLoadingTask> onLoadingComplete)
    {
        if (_isLoadComplete == false)
        {
            if (_loadingCompleteAction != null)
            {
                _loadingCompleteAction += onLoadingComplete;
            }
            else
            {
                _loadingCompleteAction = onLoadingComplete;
            }
        }
    }

    public void OnLoadComplete(AsyncOperationHandle completedHandle)
    {
        if (_isLoadComplete == false)
        {
            if (_isValid == false)
            {
                _loadingCompleteAction = null;
                _loadingFailedAction = null;
                return;
            }

            if (completedHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (_isSingleRes == false)
                {
                    _assetsDict = new Dictionary<string, UnityEngine.Object>();
                    var assets = this.GetAssets();
                    if (assets != null)
                    {
                        for (var i = 0; i < assets.Count; i++)
                        {
                            // UnityEngine.Debug.Log("AssetName : " + assets[i].name);

                            if (_assetsDict.ContainsKey(assets[i].name) == false)
                            {
                                _assetsDict.Add(assets[i].name, assets[i]);
                            }
                            else
                            {
                                Debug.Log("Duplicate Resources ： " + assets[i].name);
                            }
                        }
                    }
                }

                _loadingCompleteAction?.Invoke(this);
                _loadingCompleteAction = null;
                _loadingFailedAction = null;
                _isLoadComplete = true;
            }
            else
            {
                Release();
                _loadingFailedAction?.Invoke();
                _loadingCompleteAction = null;
                _loadingFailedAction = null;
            }
        }
    }

    public AsyncOperationHandle<T> GetHandle<T>()
    {
        return _handle.Convert<T>();
    }

    public AsyncOperationHandle GetHandle()
    {
        return _handle;
    }

    public T InstantiateAsset<T>() where T : UnityEngine.Object
    {
        if (_isSingleRes && _handle.IsValid())
        {
            return UnityEngine.Object.Instantiate((UnityEngine.Object)_handle.Result) as T;
        }

        return null;
    }

    public T GetAsset<T>(string assetAddress = null) where T : UnityEngine.Object
    {
        if (_isSingleRes && _handle.IsValid())
        {
            return _handle.Result as T;
        }

        if (!_isSingleRes && assetAddress != null)
        {
            if (_assetsDict.ContainsKey(assetAddress))
            {
                return _assetsDict[assetAddress] as T;
            }
        }

        return null;
    }

    public T InstantiateAsset<T>(string assetAddress) where T : UnityEngine.Object
    {
        Debug.Log("InstantiateAsset" + assetAddress);

        if (_isSingleRes == false)
        {
            if (_assetsDict.ContainsKey(assetAddress))
            {
                return UnityEngine.Object.Instantiate(_assetsDict[assetAddress]) as T;
            }
        }

        return null;
    }

    public void MakeInvalid()
    {
        _isValid = false;
        if (_handle.IsValid())
        {
            _handle.Completed -= OnLoadComplete;
        }
    }

    public void Release()
    {
        if (_isValid && _handle.IsValid())
        {
            _manager.ReleaseTask(this);
            MakeInvalid();
            Addressables.Release(_handle);
        }
        else
        {
            MakeInvalid();
        }
    }

    public float GetCompleteProgress()
    {
        if (_handle.IsValid())
        {
            return _handle.GetDownloadStatus().Percent;
        }
        //   Log.LogError("AssetsError", $"Invalid Handle On GetCompleteProgress({address})");
        return 0;
    }

    public IList<UnityEngine.Object> GetAssets()
    {
        if (_handle.IsValid() && _handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (_isSingleRes)
            {
                var list = new List<UnityEngine.Object>();
                list.Add((UnityEngine.Object)_handle.Result);
                return list;
            }

            if (_handle.Result is List<UnityEngine.Object>)
            {
                return (List<UnityEngine.Object>)_handle.Result;
            }
        }

        Debug.Log("빈 상태로 반환됨");

        return null;
    }
}

public class AssetsManager
{
    private List<AssetLoadingTask> taskList;
    private Dictionary<AsyncOperationHandle, Action<AsyncOperationHandle>> asyncOpDict;
    private Dictionary<AsyncOperationHandle<IList<IResourceLocation>>, Action<AsyncOperationHandle<IList<IResourceLocation>>>> checkResourceOpDict;
    private Dictionary<AsyncOperationHandle<long>, Action<AsyncOperationHandle<long>>> checkAddressOpDict;

    // 콘텐츠 다운로드 지연
    private bool isAfterEnterAssetsLoaded;

    public AssetsManager()
    {
        taskList = new List<AssetLoadingTask>();
        asyncOpDict = new Dictionary<AsyncOperationHandle, Action<AsyncOperationHandle>>();
        checkResourceOpDict = new Dictionary<AsyncOperationHandle<IList<IResourceLocation>>, Action<AsyncOperationHandle<IList<IResourceLocation>>>>();
        checkAddressOpDict = new Dictionary<AsyncOperationHandle<long>, Action<AsyncOperationHandle<long>>>();
    }

    public void Destroy()
    {
        ReleaseAllTask();

        foreach (var async in checkResourceOpDict)
        {
            async.Key.Completed -= async.Value;
        }

        foreach (var async in asyncOpDict)
        {
            async.Key.Completed -= async.Value;
        }

        foreach (var async in checkAddressOpDict)
        {
            async.Key.Completed -= async.Value;
        }

        checkResourceOpDict.Clear();
        asyncOpDict.Clear();
        checkAddressOpDict.Clear();
    }

    public AssetLoadingTask LoadAsset<T>(string address, Action<AssetLoadingTask> loadComplete, Action loadFailed = null) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(address))
        {
            loadFailed?.Invoke();
            return null;
        }

        var task = new AssetLoadingTask(address, this);
        task.LoadAsset<T>(loadComplete, loadFailed);
        taskList.Add(task);

        return task;
    }

    public AssetLoadingTask LoadAssets<T>(string address, Action<AssetLoadingTask> loadComplete) where T : UnityEngine.Object
    {
        var task = new AssetLoadingTask(address, this);
        task.LoadAssets<T>(loadComplete);
        taskList.Add(task);

        return task;
    }

    public bool CheckAddressExist<T>(string address)
    {
        foreach (var l in Addressables.ResourceLocators)
        {
            if (l.Locate(address, typeof(T), out _))
            {
                return true;
            }
        }
        return false;
    }

    public void CheckAddressDownloaded(string address, Action<bool> completeCallBack)
    {
        if (string.IsNullOrEmpty(address))
        {
            completeCallBack?.Invoke(false);
            return;
        }

        var handle = Addressables.GetDownloadSizeAsync(address);
        Action<AsyncOperationHandle<long>> completeAction = result =>
        {
            checkAddressOpDict.Remove(result);
            long size = result.Result;
            Debug.Log($"CheckAddressDownloaded: {address}: {size}");
            completeCallBack?.Invoke(size == 0);
        };
        handle.Completed += completeAction;
        checkAddressOpDict.Add(handle, completeAction);
    }

    public void CheckHasResources(string label, Action<bool> completeCallBack)
    {
        var handle = Addressables.LoadResourceLocationsAsync(label);

        Action<AsyncOperationHandle<IList<IResourceLocation>>> completeAction = (result) =>
        {
            checkResourceOpDict.Remove(result);
            IList<IResourceLocation> locations = (IList<IResourceLocation>)result.Result;
            if (locations != null && locations.Count > 0)
            {
                completeCallBack.Invoke(true);
            }
            else
            {
                completeCallBack.Invoke(false);
            }
        };

        handle.Completed += completeAction;

        checkResourceOpDict.Add(handle, completeAction);
    }

    public AsyncOperationHandle DownloadDependenciesAsync(string label, Action completeCallBack, Action failedCallBack)
    {
        Debug.Log("[AssetDownload] DownloadDependenciesAsync Begin label : " + label);

        Addressables.GetDownloadSizeAsync(label).Completed += (AsyncOperationHandle<long> SizeHandle) =>
        {
            Debug.Log("[AssetDownload] DownloadDependenciesAsync " + string.Concat(SizeHandle.Result, " byte"));
        };

        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(label, true);
        Action<AsyncOperationHandle> completeAction = (result) =>
        {
            asyncOpDict.Remove(result);

            if (result.Status == AsyncOperationStatus.Succeeded)
            {
                completeCallBack?.Invoke();
            }
            else
            {
                failedCallBack?.Invoke();
            }
        };

        asyncOpDict.Add(handle, completeAction);

        handle.Completed += completeAction;

        return handle;
    }

    public AsyncOperationHandle DownloadDependenciesAsync(List<string> label, Action completeCallBack, Action failedCallBack)
    {
        Debug.Log("[AssetDownload] DownloadDependenciesAsync Begin label : " + label.Count);

        Addressables.GetDownloadSizeAsync(label).Completed += (AsyncOperationHandle<long> SizeHandle) =>
        {
            Debug.Log("[AssetDownload] DownloadDependenciesAsync " + string.Concat(SizeHandle.Result, " byte"));
        };

        AsyncOperationHandle handle = Addressables.DownloadDependenciesAsync(label, Addressables.MergeMode.Union, true);
        Action<AsyncOperationHandle> completeAction = (result) =>
        {
            asyncOpDict.Remove(result);

            if (result.Status == AsyncOperationStatus.Succeeded)
            {
                completeCallBack?.Invoke();
            }
            else
            {
                failedCallBack?.Invoke();
            }
        };

        asyncOpDict.Add(handle, completeAction);

        handle.Completed += completeAction;

        return handle;
    }

    public void ReleaseTask(AssetLoadingTask task)
    {
        taskList.Remove(task);
    }

    public void ReleaseAllTask()
    {
        if (taskList.Count > 0)
        {
            for (var i = taskList.Count - 1; i >= 0; i--)
            {
                taskList[i].Release();
            }
            taskList.Clear();
        }
    }
}