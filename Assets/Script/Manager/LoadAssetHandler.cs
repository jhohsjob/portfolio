using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public interface ILoadAssetHandler
{
    string address { get; }

    AssetLoadingTask task { get; }

    AssetLoadingTask LoadAsset(Action<AssetLoadingTask> loadComplete = null, Action loadFailed = null);

    T GetAsset<T>() where T : UnityEngine.Object;

    void Release();
}

public class LoadAssetHandler<T> : ILoadAssetHandler where T : UnityEngine.Object
{
    public string address { get; }

    public AssetLoadingTask task { get; private set; }

    public Action<AssetLoadingTask> loadCompleteCallback { get; set; }

    public LoadAssetHandler(string address, Action<AssetLoadingTask> loadCompleteCallback = null)
    {
        this.address = address;
        this.loadCompleteCallback = loadCompleteCallback;
    }

    public AssetLoadingTask LoadAsset(Action<AssetLoadingTask> loadComplete = null, Action loadFailed = null)
    {
        if (typeof(T) == typeof(VideoClip))
        {
            Caching.compressionEnabled = false;
            task = Client.asset.LoadAsset<T>(address, task =>
            {
                Caching.compressionEnabled = true;
                loadCompleteCallback?.Invoke(task);
                loadComplete?.Invoke(task);
            }, () =>
            {
                Caching.compressionEnabled = true;
                loadFailed?.Invoke();
            });
        }
        else
        {
            task = Client.asset.LoadAsset<T>(address, task =>
            {
                loadCompleteCallback?.Invoke(task);
                loadComplete?.Invoke(task);
            }, loadFailed);
        }
        return task;
    }

    public T1 GetAsset<T1>() where T1 : UnityEngine.Object
    {
        return task?.GetAsset<T1>();
    }

    public T GetAsset()
    {
        return task?.GetAsset<T>();
    }

    public void Release()
    {
        task?.Release();
        task = null;
    }
}

public static class LoadAssetHelper
{
    public static T GetAsset<T>(this List<ILoadAssetHandler> list, string address) where T : UnityEngine.Object
    {
        if (list == null || list.Count == 0 || string.IsNullOrEmpty(address))
        {
            return null;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].address == address)
            {
                return list[i].GetAsset<T>();
            }
        }
        return null;
    }
}