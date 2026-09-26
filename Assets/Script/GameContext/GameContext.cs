using System;


public class GameContext
{
    public AssetService AssetService { get; }
    public RoleFactory RoleFactory { get; }

    public Storage Storage { get; }
    public SaveService SaveService { get; }

    public IGameServer GameServer { get; }

    public User User { get; }
    public MercenaryService MercenaryService { get; }
    public CurrencyService CurrencyService { get; }
    public PurchaseService PurchaseService { get; }
    public LocaleService LocaleService { get; }

    public StageService StageService { get; }

    public SceneService SceneService { get; }
    public PopupService PopupService { get; }

    public GameDataLoader GameDataLoader { get; }

    public GameContext()
    {
        AssetService = new AssetService();
        RoleFactory = new RoleFactory();

        Storage = new Storage(AssetService);
        SaveService = new SaveService(Storage);

        GameServer = new MockGameServer(AssetService);

        User = new User(GameServer);
        MercenaryService = new MercenaryService(RoleFactory, User, GameServer);
        CurrencyService = new CurrencyService(User, SaveService);
        PurchaseService = new PurchaseService(MercenaryService, CurrencyService);
        LocaleService = new LocaleService(Storage, SaveService);

        StageService = new StageService();

        PopupService = new PopupService(new PopupServiceDependencies
        {
            assetLoader = AssetService,
            gameServer = GameServer,
            storage = Storage
        });
        SceneService = new SceneService(new SceneServiceContext
        {
            assetLoader = AssetService,
            popupService = PopupService,
            currencyService = CurrencyService,
            stageService = StageService,
            user = User,
            mercenaryService = MercenaryService,
            purchaseService = PurchaseService,
        });

        GameDataLoader = new GameDataLoader(new GameDataLoaderContext
        {
            assetLoader = AssetService,
            gameServer = GameServer,
            user = User,
            mercenaryService = MercenaryService,
            stageService = StageService,
        });
    }

    public void Dispose()
    {
        (AssetService as IDisposable).Dispose();
        LocaleService.Dispose();
    }
}