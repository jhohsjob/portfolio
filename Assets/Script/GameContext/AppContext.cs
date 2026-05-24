//using System;

//public class AppContext
//{
//    public AssetService AssetService { get; }
//    public RoleFactory RoleFactory { get; }

//    public Storage Storage { get; }
//    public ProductStorage ProductStorage { get; }
//    public MercenaryStorage MercenaryStorage { get; }
//    public SaveService SaveService { get; }

//    public User User { get; }
//    public CurrencyService CurrencyService { get; }
//    public PurchaseService PurchaseService { get; }
//    public LocaleService LocaleService { get; }

//    public StageService StageService { get; }

//    public SceneService SceneService { get; }
//    public PopupService PopupService { get; }

//    public GameDataLoader GameDataLoader { get; }

//    public AppContext()
//    {
//        AssetService = new AssetService();
//        RoleFactory = new RoleFactory(AssetService);

//        Storage = new Storage(AssetService);
//        MercenaryStorage = new MercenaryStorage();
//        ProductStorage = new ProductStorage();
//        SaveService = new SaveService(Storage);

//        User = new User(Storage, SaveService);
//        CurrencyService = new CurrencyService(Storage, SaveService);
//        PurchaseService = new PurchaseService(CurrencyService, ProductStorage);
//        LocaleService = new LocaleService(Storage);

//        StageService = new StageService();

//        PopupService = new PopupService(this);
//        SceneService = new SceneService(this);

//        GameDataLoader = new GameDataLoader(this);
//    }

//    public void Dispose()
//    {
//        (AssetService as IDisposable).Dispose();
//        LocaleService.Dispose();
//    }
//}