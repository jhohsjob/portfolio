public class Client
{
    public static AssetsManager asset { get; private set; }
    public static Storage storage { get; private set; }
    public static MercenaryStorage mercenaryStorage { get; private set; }
    public static ProductStorage productStorage { get; private set; }
    public static User user { get; private set; }
    public static CurrencyService currencyService { get; private set; }

    //public static bool isRunGame { get; private set; }

    public Client()
    {
        asset = new AssetsManager();
        storage = new Storage();
        mercenaryStorage = new MercenaryStorage();
        productStorage = new ProductStorage();
        user = new User();
        currencyService = new CurrencyService();

        //isRunGame = false;
    }

    //public void RunGame()
    //{
    //    user.RunGame();

    //    isRunGame = true;
    //}
}
