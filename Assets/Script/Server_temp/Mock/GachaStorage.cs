using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class GachaStorage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "gacha_save.json");

    private Dictionary<int, GachaDefinition> _gachaDefinitions;

    private List<GachaPVO> _pvos = new();
    public IReadOnlyList<GachaPVO> pvos => _pvos;

    public GachaStorage()
    {
    }

    public void SetupGacha(Dictionary<int, GachaDefinition> gachas)
    {
        _gachaDefinitions = gachas;
    }

    public GachaPVO GetById(int id)
    {
        return _pvos.FirstOrDefault(x => x.id == id);
    }

    public async Task SaveAsync(List<GachaPVO> saveDatas)
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

        _pvos = JsonConvert.DeserializeObject<List<GachaPVO>>(json);
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


    private Task<List<GachaPVO>> LoadDefaultDataAsync()
    {
        var gachas = _gachaDefinitions.Values.ToList();
        var pvos = new List<GachaPVO>();
        foreach (var gacha in gachas)
        {
            pvos.Add(new GachaPVO
            {
                id = gacha.id,
                gachaCount = 0,
            });
        }

        return Task.FromResult(pvos);
    }

    private async Task LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        _pvos = JsonConvert.DeserializeObject<List<GachaPVO>>(json);

        if (_pvos == null)
        {
            throw new Exception("Save file is corrupted");
        }

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(GachaPVO changeData)
    {
        var index = _pvos.FindIndex(pvo => pvo.id == changeData.id);
        if (index == -1)
        {
            return;
        }

        _pvos[index] = changeData;

        await SaveAsync(_pvos);
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