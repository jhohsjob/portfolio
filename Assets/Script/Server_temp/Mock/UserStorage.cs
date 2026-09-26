using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class UserStorage
{
    private IAssetLoader _assetLoader;

    private static string _savePath => Path.Combine(Application.persistentDataPath, "user_save.json");

    private UserPVO _pvo = new();
    public UserPVO pvo => _pvo;

    public UserStorage(IAssetLoader assetLoader)
    {
        _assetLoader = assetLoader;
    }

    public async Task SaveAsync(UserPVO saveData)
    {
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
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

        this._pvo = JsonConvert.DeserializeObject<UserPVO>(json);
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
        _pvo = await LoadDefaultDataAsync();

        await SaveAsync(_pvo);
    }

    private Task<UserPVO> LoadDefaultDataAsync()
    {
        var tcs = new TaskCompletionSource<UserPVO>();

        _assetLoader.LoadData("UserDefaultData", data =>
        {
            var userData = data as UserDefaultData;
            if (userData == null)
            {
                tcs.SetException(new Exception("UserDefaultData load failed"));
                return;
            }

            var pvo = new UserPVO
            {
                id = 1001,
                name = "Default User",
                level = userData.level,
                exp = userData.exp,
                gold = userData.gold,
                lastPlayMercenaryId = userData.mercenaryId,
                lastPlayStageId = userData.stageId,
            };

            tcs.SetResult(pvo);
        });

        return tcs.Task;
    }

    private async Task LoadFromDisk()
    {
        string json = File.ReadAllText(_savePath);
        _pvo = JsonConvert.DeserializeObject<UserPVO>(json);

        if (_pvo == null)
        {
            throw new Exception("Save file is corrupted");
        }

        await Task.CompletedTask;
    }

    public async Task Update(UserPVO changeData)
    {
        _pvo = changeData;

        await SaveAsync(_pvo);
    }

#if UNITY_EDITOR
    [MenuItem("CustomMenu/DataDelete_user")]
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