using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


// 프리로드 에셋은 세션 동안 상주하므로 GameContext가 소유한다.
public sealed class PreloadCache : IDisposable
{
    private readonly List<AsyncOperationHandle<UnityEngine.Object>> _handles = new();

    public IReadOnlyList<AsyncOperationHandle<UnityEngine.Object>> Handles => _handles;

    public void Add(AsyncOperationHandle<UnityEngine.Object> handle) => _handles.Add(handle);

    public void Release()
    {
        foreach (var h in _handles)
        {
            if (h.IsValid()) Addressables.Release(h);
        }
        _handles.Clear();
    }

    public void Dispose() => Release();
}

public class GameContext : IDisposable
{
    public IAssetService AssetService { get; }
    public Storage Storage { get; }
    public SaveService SaveService { get; }
    public ILocaleService LocaleService { get; }
    public ISoundService SoundService { get; }
    public IDeviceSettings DeviceSettings { get; }

    public PreloadCache Preload { get; }
    public IGameServer GameServer { get; }
    public User User { get; }
    public ICurrencyService CurrencyService { get; }
    public RoleFactory RoleFactory { get; }
    public MercenaryService MercenaryService { get; }
    public MonsterCatalog MonsterCatalog { get; }
    public ProjectileCatalog ProjectileCatalog { get; }
    public DropItemCatalog DropItemCatalog { get; }
    public PurchaseService PurchaseService { get; }
    public GachaService GachaService { get; }
    public StageService StageService { get; }
    public ISceneService SceneService { get; }
    public IPopupService PopupService { get; }
    public GameDataLoader GameDataLoader { get; }

    private bool _disposed;

    public GameContext(AppContext app)
    {
        AssetService = new AssetService();
        Storage = app.Storage;
        SaveService = app.SaveService;
        LocaleService = app.LocaleService;
        SoundService = app.SoundService;
        DeviceSettings = app.DeviceSettings;

        Preload = new PreloadCache();

        GameServer = new MockGameServer(AssetService);

        User = new User(GameServer);

        CurrencyService = new CurrencyService(User);

        RoleFactory = new RoleFactory();

        MercenaryService = new MercenaryService(new MercenaryService.Deps
        {
            factory = RoleFactory,
            gameServer = GameServer,
            user = User,
            currencyService = CurrencyService
        });

        MonsterCatalog = new MonsterCatalog(RoleFactory);
        
        ProjectileCatalog = new ProjectileCatalog(RoleFactory);

        DropItemCatalog = new DropItemCatalog(RoleFactory);

        PurchaseService = new PurchaseService(new PurchaseService.Deps
        {
            gameServer = GameServer,
            user = User,
            currencyService = CurrencyService
        });

        GachaService = new GachaService(new GachaService.Deps
        {
            gameServer = GameServer,
            user = User,
            currencyService = CurrencyService,
            mercenaryService = MercenaryService
        });

        StageService = new StageService();

        PopupService = new PopupService(new PopupService.Deps
        {
            assetService = AssetService,
            soundService = SoundService,
            container = app.PopupContainer
        });

        SceneService = new SceneService(new SceneService.Deps
        {
            assetService = AssetService,
            localeService = LocaleService,
            soundService = SoundService,
            deviceSettings = DeviceSettings,
            user = User,
            currencyService = CurrencyService,
            mercenaryService = MercenaryService,
            purchaseService = PurchaseService,
            gachaService = GachaService,
            stageService = StageService,
            popupService = PopupService,

            monsterCatalog = MonsterCatalog,
            projectileCatalog = ProjectileCatalog,
            dropItemCatalog = DropItemCatalog,
        });

        GameDataLoader = new GameDataLoader(new GameDataLoader.Deps
        {
            assetService = AssetService,
            soundService = SoundService,
            gameServer = GameServer,
            user = User,
            mercenaryService = MercenaryService,
            purchaseService = PurchaseService,
            gachaService = GachaService,
            stageService = StageService,

            monsterCatalog = MonsterCatalog,
            projectileCatalog = ProjectileCatalog,
            dropItemCatalog = DropItemCatalog,
        });
    }

    public void Dispose()
    {
        if (_disposed == true)
        {
            return;
        }
        _disposed = true;

        SceneService?.Dispose();
        PopupService?.Dispose();
        GachaService?.Dispose();
        GameServer?.Dispose();
        MercenaryService?.Dispose();
        CurrencyService?.Dispose();
        Preload?.Dispose();
        AssetService?.Dispose();
    }
}