public class Client
{
    public static AssetsManager asset { get; private set; }
    public static Storage storage { get; private set; }
    public static User user { get; private set; }

    public static bool isRunGame { get; private set; }

    public Client()
    {
        asset = new AssetsManager();
        storage = new Storage();
        user = new User();

        isRunGame = false;
    }

    public void RunGame()
    {
        user.RunGame();

        isRunGame = true;
    }
}
