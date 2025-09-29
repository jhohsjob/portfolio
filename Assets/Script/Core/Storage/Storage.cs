using System;
using System.IO;
using UnityEditor;
using UnityEngine;


public class Storage
{
    private static string _savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public GameSaveData data { get; private set; }

    public Storage()
    {
    }

    public void Save(GameSaveData data)
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
            
            this.data = data;
        }
        catch (Exception ex)
        {
            Debug.LogError("Storage Save Failed : " + ex);
        }
    }

    public void Load(Action callback)
    {
        try
        {
            if (File.Exists(_savePath) == false)
            {
                Client.asset.LoadAsset<UserDefaultData>("UserDefaultData", (task) =>
                {
                    var data = new GameSaveData
                    {
                        player = new UserData(task.GetAsset<UserDefaultData>())
                    };

                    Save(data);

                    callback?.Invoke();
                });
            }
            else
            {
                string json = File.ReadAllText(_savePath);
                data = JsonUtility.FromJson<GameSaveData>(json);

                callback?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Storage Load Failed : " + ex);
        }
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