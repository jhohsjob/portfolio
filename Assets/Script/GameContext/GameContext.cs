using System;


public class GameContext
{
    public AssetService AssetService { get; }
    public RoleFactory RoleFactory { get; }

    public Storage Storage { get; }
    public ProductStorage ProductStorage { get; }
    public MercenaryStorage MercenaryStorage { get; }
    public SaveService SaveService { get; }

    public User User { get; }
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
        MercenaryStorage = new MercenaryStorage();
        ProductStorage = new ProductStorage();
        SaveService = new SaveService(Storage);

        User = new User(Storage, SaveService);
        CurrencyService = new CurrencyService(Storage, SaveService);
        PurchaseService = new PurchaseService(CurrencyService, ProductStorage);
        LocaleService = new LocaleService(Storage, SaveService);

        StageService = new StageService();

        PopupService = new PopupService(new PopupServiceDependencies
        {
            AssetLoader = AssetService,
            Storage = Storage
        });
        SceneService = new SceneService(new SceneServiceContext
        {
            assetLoader = AssetService,
            popupService = PopupService,
            currencyService = CurrencyService,
            stageService = StageService,
            user = User,
            productStorage = ProductStorage,
            purchaseService = PurchaseService,
        });

        GameDataLoader = new GameDataLoader(new GameDataLoaderContext
        {
            assetLoader = AssetService,
            stageService = StageService,
        });
    }

    public void Dispose()
    {
        (AssetService as IDisposable).Dispose();
        LocaleService.Dispose();
    }
}