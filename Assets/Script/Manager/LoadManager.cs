using System;


public class LoadManager
{
    public static void Load(Action callback)
    {
        LoadData(() => LoadStorage(() => LoadComplete(callback)));
    }

    private static void LoadData(Action callback)
    {
        DataManager.Load(() => { callback?.Invoke(); });
    }

    private static void LoadStorage(Action callback)
    {
        Client.storage.Load(() => { callback?.Invoke(); });
    }

    private static void LoadComplete(Action callback)
    {
        callback?.Invoke();
    }
}
