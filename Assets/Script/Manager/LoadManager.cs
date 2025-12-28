using System.Threading.Tasks;


public static class LoadManager
{
    public static async Task LoadAsync(System.Action<float> onProgress)
    {
        float progress = 0f;
        float step = 1f / 2f;

        await DataManager.LoadAsync();
        progress += step;
        onProgress?.Invoke(progress);

        await Client.storage.LoadAsync();
        progress += step;
        onProgress?.Invoke(progress);
    }
}