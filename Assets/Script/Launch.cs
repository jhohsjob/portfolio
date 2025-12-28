using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


public class Launch : MonoBehaviour
{
    private BootController _bootController;

    private const float ADDRESSABLE_INIT_WEIGHT = 0.3f;
    private const float ADDRESSABLE_LOAD_WEIGHT = 0.4f;
    private const float LOAD_WEIGHT = 0.3f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Initialize(BootController controller)
    {
        _bootController = controller;
    }

    public async Task RunAsync()
    {
        try
        {
            InitializeClient();
            await InitializeAddressablesAsync();
            await LoadPreloadAssetsAsync();
            await InitializeLoadAsync();

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
        new Client();
        PopupManager.Initialization();
    }

    private async Task InitializeAddressablesAsync()
    {
        var handle = Addressables.InitializeAsync();
        await TrackProgressAsync(handle, 0f, ADDRESSABLE_INIT_WEIGHT);
    }

    private async Task LoadPreloadAssetsAsync()
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

        await TrackProgressAsync(handles, ADDRESSABLE_INIT_WEIGHT, ADDRESSABLE_LOAD_WEIGHT);
    }

    private async Task InitializeLoadAsync()
    {
        await LoadManager.LoadAsync(progress =>
        {
            _bootController.SetProgress(ADDRESSABLE_INIT_WEIGHT + ADDRESSABLE_LOAD_WEIGHT + progress * LOAD_WEIGHT);
        });
    }

    private async Task TrackProgressAsync(AsyncOperationHandle handle, float start, float range)
    {
        while (handle.IsDone == false)
        {
            _bootController.SetProgress(start + handle.PercentComplete * range);
            await Task.Yield();
        }

        _bootController.SetProgress(start + range);
    }

    private async Task TrackProgressAsync<T>(List<AsyncOperationHandle<T>> handles, float start, float range)
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

            _bootController.SetProgress(start + (sum / handles.Count) * range);

            if (done)
            {
                break;
            }

            await Task.Yield();
        }

        _bootController.SetProgress(start + range);
    }

    private void Complete()
    {
        Client.user.RunGame();
    }
}
