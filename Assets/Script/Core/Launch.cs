using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class Launch : MonoBehaviour
{
    public static GameContext context { get; private set; }
    public GameContext Context => context;

    //public static AppContext appContext { get; private set; }
    //public AppContext AppContext => appContext;

    private const float ADDRESSABLE_INIT_WEIGHT = 0.25f;
    private const float LOCALIZATION_INIT_WEIGHT = 0.1f;
    private const float ADDRESSABLE_LOAD_WEIGHT = 0.35f;
    private const float LOAD_WEIGHT = 0.3f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        context = new GameContext();
        //appContext = new AppContext();
    }

    private void OnDestroy()
    {
        context.Dispose();
    }

    public async Task RunAsync(BootController bootController)
    {
        try
        {
            InitializeClient();
            await InitializeAddressablesAsync(bootController);
            await InitializeLocalizationAsync(bootController);
            await LoadPreloadAssetsAsync(bootController);
            await InitializeLoadAsync(bootController);

            Complete();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }
    }

    private void InitializeClient()
    {
        MercenaryManager.instance.Init(context.RoleFactory, context.MercenaryStorage);
        MonsterManager.instance.Init(context.RoleFactory);
        ProjectileManager.instance.Init(context.RoleFactory);
        DropItemManager.instance.Init(context.RoleFactory);
    }

    private async Task InitializeAddressablesAsync(BootController bootController)
    {
        var handle = Addressables.InitializeAsync();
        await TrackProgressAsync(bootController, handle, 0f, ADDRESSABLE_INIT_WEIGHT);
    }

    private async Task InitializeLocalizationAsync(BootController bootController)
    {
        var handle = LocalizationSettings.InitializationOperation;

        await TrackProgressAsync(bootController, handle, ADDRESSABLE_INIT_WEIGHT, LOCALIZATION_INIT_WEIGHT);
    }

    private async Task LoadPreloadAssetsAsync(BootController bootController)
    {
        string[] labels = { "Preload" };

        var locations = new List<IResourceLocation>();

        foreach (var label in labels)
        {
            var handle = Addressables.LoadResourceLocationsAsync(label);
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"Preload label failed: {label}");
                continue;
            }

            locations.AddRange(handle.Result);
        }

        locations = locations.Distinct().ToList();

        if (locations.Count == 0)
        {
            return;
        }

        var handles = locations.Select(loc => Addressables.LoadAssetAsync<UnityEngine.Object>(loc)).ToList();

        await TrackProgressAsync(bootController, handles, ADDRESSABLE_INIT_WEIGHT + LOCALIZATION_INIT_WEIGHT, ADDRESSABLE_LOAD_WEIGHT);
    }

    private async Task InitializeLoadAsync(BootController bootController)
    {
        float progress = 0f;
        float step = 1f / 4f;

        await context.GameDataLoader.LoadAsync();

        progress += step;
        bootController.SetProgress(ADDRESSABLE_INIT_WEIGHT + LOCALIZATION_INIT_WEIGHT + ADDRESSABLE_LOAD_WEIGHT + progress * LOAD_WEIGHT);

        await context.Storage.LoadAsync();

        progress += step;
        bootController.SetProgress(ADDRESSABLE_INIT_WEIGHT + LOCALIZATION_INIT_WEIGHT + ADDRESSABLE_LOAD_WEIGHT + progress * LOAD_WEIGHT);

        await context.MercenaryStorage.LoadAsync();

        progress += step;
        bootController.SetProgress(ADDRESSABLE_INIT_WEIGHT + LOCALIZATION_INIT_WEIGHT + ADDRESSABLE_LOAD_WEIGHT + progress * LOAD_WEIGHT);

        await context.ProductStorage.LoadAsync();
        
        progress += step;
        bootController.SetProgress(ADDRESSABLE_INIT_WEIGHT + LOCALIZATION_INIT_WEIGHT + ADDRESSABLE_LOAD_WEIGHT + progress * LOAD_WEIGHT);
    }

    private async Task TrackProgressAsync(BootController bootController, AsyncOperationHandle handle, float start, float range)
    {
        while (handle.IsDone == false)
        {
            bootController.SetProgress(start + handle.PercentComplete * range);
            await Task.Yield();
        }

        bootController.SetProgress(start + range);
    }

    private async Task TrackProgressAsync<T>(BootController bootController, List<AsyncOperationHandle<T>> handles, float start, float range)
    {
        while (true)
        {
            float sum = 0f;
            bool done = true;

            foreach (var handle in handles)
            {
                sum += handle.PercentComplete;
                if (handle.IsDone == false)
                {
                    done = false;
                }
            }

            bootController.SetProgress(start + (sum / handles.Count) * range);

            if (done)
            {
                break;
            }

            await Task.Yield();
        }

        bootController.SetProgress(start + range);
    }

    private void Complete()
    {
        context.LocaleService.Init();
        context.CurrencyService.Init();
        context.User.RunGame();
    }
}