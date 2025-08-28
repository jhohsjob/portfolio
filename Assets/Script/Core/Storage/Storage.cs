using System.IO;
using UnityEngine;


public class Storage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static GameSaveData Data { get; private set; }

    public static void Save(GameSaveData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            string tempPath = _savePath + ".tmp";
            
            File.WriteAllText(tempPath, json);

            if (File.Exists(_savePath))
            {
                File.Delete(_savePath);
            }
            
            File.Move(tempPath, _savePath);
            
            Data = data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("세이브 실패: " + ex);
        }
    }

    public static void Load()
    {
        try
        {
            if (File.Exists(_savePath) == false)
            {
                var userDefault = Resources.Load<UserDefaultData>("DefaultStoage/UserDefaultData");
                var data = new GameSaveData
                {
                    player = new UserData(userDefault)
                };

                Save(data);
                return;
            }

            string json = File.ReadAllText(_savePath);
            Data = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("로드 실패: " + ex);
            Data = new GameSaveData();
        }
    }

    public static void Delete()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
        }
    }
}