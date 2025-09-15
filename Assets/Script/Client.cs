public class Client
{
    public static AssetsManager asset { get; private set; }
    public static Storage storage { get; private set; }
    public static User user { get; private set; }

    public Client()
    {
        asset = new AssetsManager();
        storage = new Storage();
        user = new User();
    }

    public void RunGame()
    {
        storage.RunGame();
        user.RunGame();

        PopupManager.Initialization();

        LoadManager.Load();

        EventHelper.Send(EventName.LoadEnd, this);
    }
}
