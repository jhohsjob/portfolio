using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class MercenarySaveData
{
    public int id;
    public bool isOwned;

    public MercenarySaveData() { }

    public MercenarySaveData(MercenarySaveData other)
    {
        id = other.id;
        isOwned = other.isOwned;
    }
}

public class MercenaryStorage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "mercenary_save.json");

    private Dictionary<int, MercenarySaveData> _saveDatas = new();

    public MercenaryStorage()
    {
    }

    public async Task SaveAsync(Dictionary<int, MercenarySaveData> saveDatas)
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

        this._saveDatas = JsonConvert.DeserializeObject<Dictionary<int, MercenarySaveData>>(json);
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

            MercenaryManager.instance.ApplySaveData(_saveDatas);
        }
        catch
        {
            throw;
        }
    }

    private async Task CreateDefaultSaveAsync()
    {
        _saveDatas = await LoadDefaultDataAsync();

        await SaveAsync(_saveDatas);
    }

    private async Task LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        _saveDatas = JsonConvert.DeserializeObject<Dictionary<int, MercenarySaveData>>(json);

        if (_saveDatas == null)
        {
            throw new Exception("Save file is corrupted");
        }

        await Task.CompletedTask;
    }

    private Task<Dictionary<int, MercenarySaveData>> LoadDefaultDataAsync()
    {
        var mercenaries = MercenaryManager.instance.list;
        var saveDatas = new Dictionary<int, MercenarySaveData>(mercenaries.Count);
        foreach (var mercenary in mercenaries)
        {
            saveDatas.Add(mercenary.id, new MercenarySaveData
            {
                id = mercenary.id,
                isOwned = false
            });
        }

        saveDatas[101001].isOwned = true;

        return Task.FromResult(saveDatas);
    }

    public async Task Update(MercenarySaveData changeData)
    {
        if (_saveDatas.ContainsKey(changeData.id) == false)
        {
            return;
        }

        _saveDatas[changeData.id] = changeData;

        await SaveAsync(_saveDatas);

        MercenaryManager.instance.ApplySaveData(_saveDatas);
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_mercenary")]
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