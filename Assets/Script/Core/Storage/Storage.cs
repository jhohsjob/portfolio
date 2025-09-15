using System.IO;
using UnityEngine;


public class Storage
{
    private string _savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public GameSaveData data { get; private set; }

    public Storage()
    {
    }

    public void RunGame()
    {
        Load();
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
        catch (System.Exception ex)
        {
            Debug.LogError("save failed : " + ex);
        }
    }

    public void Load()
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
                });
                return;
            }

            string json = File.ReadAllText(_savePath);
            data = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("load failed : " + ex);
            data = new GameSaveData();
        }
    }

    public void Delete()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
        }
    }
}