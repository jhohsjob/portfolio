using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class ProductStorage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "product_save.json");

    private Dictionary<int, ProductDefinition> _productDefinitions;

    private List<ProductPVO> _pvos = new();
    public IReadOnlyList<ProductPVO> pvos => _pvos;

    public ProductStorage()
    {
    }

    public void SetupProduct(Dictionary<int, ProductDefinition> products)
    {
        _productDefinitions = products;
    }

    public ProductPVO GetById(int id)
    {
        return _pvos.FirstOrDefault(x => x.id == id);
    }

    public async Task SaveAsync(List<ProductPVO> saveDatas)
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

        _pvos = JsonConvert.DeserializeObject<List<ProductPVO>>(json);
    }

    public async Task LoadAsync()
    {
        string savePath = _savePath;

        try
        {
            if (File.Exists(savePath) == false)
            {
                await CreateDefaultSaveAsync();
            }
            else
            {
                await LoadFromDisk();
            }
        }
        catch
        {
            throw;
        }
    }

    private async Task CreateDefaultSaveAsync()
    {
        _pvos = await LoadDefaultDataAsync();

        await SaveAsync(_pvos);
    }


    private Task<List<ProductPVO>> LoadDefaultDataAsync()
    {
        var products = _productDefinitions.Values.ToList();
        var pvos = new List<ProductPVO>();
        foreach (var product in products)
        {
            pvos.Add(new ProductPVO
            {
                id = product.id,
                purchaseCount = 0,
                lastPurchaseTime = 0,
                lastResetTime = 0
            });
        }

        return Task.FromResult(pvos);
    }

    private async Task LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        _pvos = JsonConvert.DeserializeObject<List<ProductPVO>>(json);

        if (_pvos == null)
        {
            throw new Exception("Save file is corrupted");
        }

        foreach (var data in _pvos)
        {
            ResetIfNeeded(data);
        }

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(ProductPVO changeData)
    {
        var index = _pvos.FindIndex(pvo => pvo.id == changeData.id);
        if (index == -1)
        {
            return;
        }

        _pvos[index] = changeData;

        await SaveAsync(_pvos);
    }

    private void ResetIfNeeded(ProductPVO data)
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