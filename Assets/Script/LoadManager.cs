public class LoadManager
{
    private static bool isLoad = false;

    public static void Load()
    {
        if (isLoad == true)
        {
            return;
        }
        isLoad = true;

        DataManager.Load();
        Storage.Load();
    }
}
