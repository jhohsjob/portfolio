using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class Storage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public GameSaveData data { get; private set; }

    public Storage()
    {
    }

    public async Task SaveAsync(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
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

        this.data = JsonUtility.FromJson<GameSaveData>(json);
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
                LoadFromDisk();
            }
        }
        catch
        {
            throw;
        }
    }

    private async Task CreateDefaultSaveAsync()
    {
        var defaultData = await LoadDefaultDataAsync();

        data = new GameSaveData
        {
            player = new UserData(defaultData)
        };

        await SaveAsync(data);
    }

    private void LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        data = JsonUtility.FromJson<GameSaveData>(json);

        if (data == null)
        {
            throw new Exception("Save file is corrupted");
        }
    }

    private Task<UserDefaultData> LoadDefaultDataAsync()
    {
        var tcs = new TaskCompletionSource<UserDefaultData>();

        Client.asset.LoadAsset<UserDefaultData>("UserDefaultData", task =>
        {
            var asset = task.GetAsset<UserDefaultData>();
            if (asset == null)
            {
                tcs.SetException(new Exception("UserDefaultData load failed"));
                return;
            }

            tcs.SetResult(asset);
        });

        return tcs.Task;
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete")]
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