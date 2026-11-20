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
    public LocaleService LocaleService { get; }
    public ISoundService SoundService { get; }
    public IDeviceSettings DeviceSettings { get; }

    public PreloadCache Preload { get; }
    public IGameServer GameServer { get; }
    public User User { get; }
    public ICurrencyService CurrencyService { get; }
    public RoleFactory RoleFactory { get; }
    public MercenaryService MercenaryService { get; }
    public PurchaseService PurchaseService { get; }
    public GachaService GachaService { get; }
    public StageService StageService { get; }
    public ISceneService SceneService { get; }
    public IPopupService PopupService { get; }
    public GameDataLoader GameDataLoader { get; }

    public MonsterManager MonsterManager { get; }
    public ProjectileManager ProjectileManager { get; }
    public DropItemManager DropItemManager { get; }

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


        MonsterManager = new MonsterManager(RoleFactory);
        ProjectileManager = new ProjectileManager(RoleFactory);
        DropItemManager = new DropItemManager(RoleFactory);


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

        StageService = new StageService();

        PopupService = new PopupService(new PopupServiceDependencies
        {
            assetService = AssetService,
            soundService = SoundService,
            container = app.PopupContainer
        });

        SceneService = new SceneService(new SceneService.Deps
        {
            assetService = AssetService,
            soundService = SoundService,
            deviceSettings = DeviceSettings,
            user = User,
            currencyService = CurrencyService,
            mercenaryService = MercenaryService,
            purchaseService = PurchaseService,
            gachaService = GachaService,
            stageService = StageService,
            popupService = PopupService,

            monsterManager = MonsterManager,
            projectileManager = ProjectileManager,
            dropItemManager = DropItemManager,
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

            monsterManager = MonsterManager,
            projectileManager = ProjectileManager,
            dropItemManager = DropItemManager,
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
        GameServer?.Dispose();
        Preload.Dispose();
    }
}