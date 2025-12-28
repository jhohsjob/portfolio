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
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string savePath = _savePath; // UnityException: get_persistentDataPath can only be called from the main thread.
            string tempPath = savePath + ".tmp";

            await Task.Run(() =>
            {
                File.WriteAllText(tempPath, json);

                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }

                File.Move(tempPath, savePath);
            });

            this.data = data;
        }
        catch
        {
            throw;
        }
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

    [MenuItem("CustomMenu/DataDelete")]
    public static void Delete()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
        }

        Debug.Log("delete complete");
    }
}