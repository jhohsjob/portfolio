using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class ProductSaveData
{
    public int id;
    public int purchaseCount;
    public long lastPurchaseTime;
    public long lastResetTime;

    public ProductSaveData() { }

    public ProductSaveData(ProductSaveData other)
    {
        id = other.id;
        purchaseCount = other.purchaseCount;
        lastPurchaseTime = other.lastPurchaseTime;
        lastResetTime = other.lastResetTime;
    }
}


public class ProductStorage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "product_save.json");

    private Dictionary<int, ProductSaveData> _saveDatas = new();

    public ProductStorage()
    {
    }

    public async Task SaveAsync(Dictionary<int, ProductSaveData> saveDatas)
    {
        string json = JsonConvert.SerializeObject(saveDatas, Formatting.Indented);
        string savePath = _savePath; // UnityException: get_persistentDataPath can only be called from the main thread.
        string tempPath = savePath + ".tmp";

        try
        {
            await using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            await using (var writer = new StreamWriter(fs))
            {
                await writer.WriteAsync(json);
            }

            if (File.Exists(savePath))
            {
                File.Replace(tempPath, savePath, null);
            }
            else
            {
                File.Move(tempPath, savePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e}");
            throw;
        }

        this._saveDatas = JsonConvert.DeserializeObject<Dictionary<int, ProductSaveData>>(json);
    }

    public async Task LoadAsync()
    {
        string savePath = _savePath;

        try
        {
            if (File.Exists(savePath))
            {
                await LoadFromDisk();
            }
        }
        catch
        {
            throw;
        }
    }

    private async Task LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        _saveDatas = JsonConvert.DeserializeObject<Dictionary<int, ProductSaveData>>(json);

        if (_saveDatas == null)
        {
            throw new Exception("Save file is corrupted");
        }

        foreach (var data in _saveDatas.Values)
        {
            ResetIfNeeded(data);
        }

        await Task.CompletedTask;
    }

    public async Task Update(ProductSaveData changeData)
    {
        if (_saveDatas.ContainsKey(changeData.id) == false)
        {
            return;
        }
        _saveDatas[changeData.id] = changeData;

        await SaveAsync(_saveDatas);
    }

    public ProductSaveData Get(int id)
    {
        if (_saveDatas.TryGetValue(id, out var saveData) == true)
        {
            return new ProductSaveData(saveData);
        }

        var data = new ProductSaveData
        {
            id = id,
            purchaseCount = 0,
            lastPurchaseTime = 0,
            lastResetTime = 0
        };
        _saveDatas[id] = data;
        return data;
    }

    private void ResetIfNeeded(ProductSaveData data)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (data.lastResetTime == 0)
        {
            data.lastResetTime = now;
            return;
        }

        if (TimeUtil.IsSameDayUTC(data.lastResetTime, now) == false)
        {
            data.purchaseCount = 0;
            data.lastResetTime = now;
        }
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_product")]
    public static void Delete()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
        }

        Debug.Log("delete complete");
    }
#endif
}