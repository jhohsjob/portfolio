using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameDataLoader
{
    public class Deps
    {
        public IAssetService assetService;
        public ISoundService soundService;
        public IGameServer gameServer;
        public PurchaseService purchaseService;
        public User user;
        public MercenaryService mercenaryService;
        public StageService stageService;
        public GachaService gachaService;

        public MonsterCatalog monsterCatalog;
        public ProjectileCatalog projectileCatalog;
        public DropItemCatalog dropItemCatalog;
    }


    private Deps _deps;

    public GameDataLoader(Deps context)
    {
        _deps = context;
    }

    public async UniTask LoadAsync()
    {
        var tasks = new List<UniTask>
        {
            LoadTableAsync("SoundDefinitionTable", asset => _deps.soundService.SetupDefinition(((SoundDefinitionTable)asset).table)),

            LoadTableAsync("UserLevelTable", asset => _deps.gameServer.SetupLevels(((UserLevelTable)asset).userLevels)),

            LoadTableAsync("MercenaryLevelTable", asset =>
            {
                _deps.gameServer.SetupMercenaryLevels(((MercenaryLevelTable)asset).mercenaryLevels);
                _deps.mercenaryService.SetupLevels(((MercenaryLevelTable)asset).mercenaryLevels);
            }),

            LoadTableAsync("MercenaryGradeTable", asset =>
            {
                _deps.gameServer.SetupMercenaryGrades(((MercenaryGradeTable)asset).mercenaryGrades);
                _deps.mercenaryService.SetupGrades(((MercenaryGradeTable)asset).mercenaryGrades);
            }),

            LoadTableAsync("MercenaryDefinitionTable", asset =>
            {
                _deps.gameServer.SetupMercenaries(((MercenaryDefinitionTable)asset).table);
                _deps.mercenaryService.SetupDefinition(((MercenaryDefinitionTable)asset).table);
            }),

            LoadTableAsync("MonsterDefinitionTable", asset => _deps.monsterCatalog.Setup(((MonsterDefinitionTable)asset).table)),

            LoadTableAsync("ProjectileDefinitionTable", asset => _deps.projectileCatalog.Setup(((ProjectileDefinitionTable)asset).table)),

            LoadTableAsync("DropItemDefinitionTable", asset => _deps.dropItemCatalog.Setup(((DropItemDefinitionTable)asset).table)),

            LoadTableAsync("StageDefinitionTable", asset => _deps.stageService.Init(((StageDefinitionTable)asset).table)),

            LoadTableAsync("ShopItemDefinitionTable", asset =>
            {
                _deps.purchaseService.SetupShopItems(((ShopItemDefinitionTable)asset).table);
                _deps.gameServer.SetupShopItems(((ShopItemDefinitionTable)asset).table);
            }),

            LoadTableAsync("GachaDefinitionTable", asset =>
            {
                _deps.gameServer.SetupGacha(((GachaDefinitionTable)asset).table);
                _deps.gachaService.SetupDefinition(((GachaDefinitionTable)asset).table);
            }),
        };

        await UniTask.WhenAll(tasks);
    }

    private UniTask LoadTableAsync(string address, Action<ScriptableObject> onLoaded)
    {
        var tcs = new UniTaskCompletionSource<bool>();

        _deps.assetService.LoadData(address, data =>
        {
            try
            {
                if (data == null)
                {
                    throw new Exception($"Data Load Failed: {address}");
                }

                onLoaded?.Invoke(data);
                tcs.TrySetResult(true);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        },
        () => tcs.TrySetException(new Exception($"Data Load Failed: {address}")));

        return tcs.Task;
    }
}