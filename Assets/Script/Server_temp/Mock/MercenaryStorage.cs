using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class MercenaryStorage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "mercenary_save.json");

    private Dictionary<int, MercenaryDefinition> _mercenaryDefinitions;

    private List<MercenaryPVO> _pvos = new();
    public IReadOnlyList<MercenaryPVO> pvos => _pvos;

    public MercenaryStorage()
    {
    }

    public void SetupMercenaries(Dictionary<int, MercenaryDefinition> mercenaries)
    {
        _mercenaryDefinitions = mercenaries;
    }

    public MercenaryPVO GetById(int id)
    {
        return _pvos.FirstOrDefault(x => x.id == id);
    }

    public async Task SaveAsync(List<MercenaryPVO> saveDatas)
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

        _pvos = JsonConvert.DeserializeObject<List<MercenaryPVO>>(json);
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

    private Task<List<MercenaryPVO>> LoadDefaultDataAsync()
    {
        var mercenaries = _mercenaryDefinitions.Values.ToList();
        var pvos = new List<MercenaryPVO>();
        foreach (var mercenary in mercenaries)
        {
            pvos.Add(new MercenaryPVO
            {
                id = mercenary.id,
                isOwned = false,
                level = 1,
                grade = 0,
                useGold = 0
            });
        }

        pvos.Find(x => _mercenaryDefinitions.ContainsKey(x.id)  == true &&
                       _mercenaryDefinitions[x.id].acquire.type == MercenaryAcquireType.Default)
            .isOwned = true;

        return Task.FromResult(pvos);
    }

    private async Task LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        _pvos = JsonConvert.DeserializeObject<List<MercenaryPVO>>(json);

        if (_pvos == null)
        {
            throw new Exception("Save file is corrupted");
        }

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(MercenaryPVO changeData)
    {
        var index = _pvos.FindIndex(m => m.id == changeData.id);
        if (index == -1)
        {
            return;
        }

        _pvos[index] = changeData;

        await SaveAsync(_pvos);
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