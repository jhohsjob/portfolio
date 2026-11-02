using System;


public class GameContext
{
    public IAssetService AssetService { get; }
    public Storage Storage { get; }
    public SaveService SaveService { get; }
    public IGameServer GameServer { get; }
    public User User { get; }
    public CurrencyService CurrencyService { get; }
    public RoleFactory RoleFactory { get; }
    public MercenaryService MercenaryService { get; }
    public PurchaseService PurchaseService { get; }
    public GachaService GachaService { get; }
    public LocaleService LocaleService { get; }
    public StageService StageService { get; }
    public SceneService SceneService { get; }
    public PopupService PopupService { get; }
    public GameDataLoader GameDataLoader { get; }

    public GameContext()
    {
        AssetService = new AssetService();

        Storage = new Storage(AssetService);

        SaveService = new SaveService(Storage);

        GameServer = new MockGameServer(AssetService);

        User = new User(GameServer);

        CurrencyService = new CurrencyService(User);

        RoleFactory = new RoleFactory();

        MercenaryService = new MercenaryService(new MercenaryServiceDependencies
        {
            factory = RoleFactory,
            gameServer = GameServer,
            user = User,
            currencyService = CurrencyService
        });

        PurchaseService = new PurchaseService(new PurchaseServiceDependencies
        {
            gameServer = GameServer,
            user = User,
            currencyService = CurrencyService
        });

        GachaService = new GachaService(new GachaServiceDependencies
        {
            gameServer = GameServer,
            user = User,
            currencyService = CurrencyService,
            mercenaryService = MercenaryService
        });

        LocaleService = new LocaleService(Storage, SaveService);

        StageService = new StageService();

        PopupService = new PopupService(new PopupServiceDependencies
        {
            assetService = AssetService,
            storage = Storage,
            gameServer = GameServer,
        });

        SceneService = new SceneService(new SceneServiceContext
        {
            assetService = AssetService,
            user = User,
            currencyService = CurrencyService,
            mercenaryService = MercenaryService,
            purchaseService = PurchaseService,
            gachaService = GachaService,
            stageService = StageService,
            popupService = PopupService,
        });

        GameDataLoader = new GameDataLoader(new GameDataLoaderContext
        {
            assetService = AssetService,
            gameServer = GameServer,
            user = User,
            mercenaryService = MercenaryService,
            purchaseService = PurchaseService,
            gachaService = GachaService,
            stageService = StageService,
        });
    }

    public void Dispose()
    {
        (AssetService as IDisposable).Dispose();
        LocaleService.Dispose();
    }
}