using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class Launch : MonoBehaviour
{
    private static Launch _instance;

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        var client = new Client();

        PopupManager.Initialization();

        InitializeAddressable(() => InitializeLoad(() => InitializeComplete(client)));
    }

    private void InitializeAddressable(Action callback)
    {
        Addressables.InitializeAsync().Completed += (data) =>
        {
            Debug.Log("InitializeAddressable Status : " + data.Status + ", OperationException :  " + data.OperationException);

            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("InitializeAddressable Succeeded LocatorId : " + data.Result.LocatorId);

                callback?.Invoke();
            }
            else
            {
                Debug.Log("InitializeAddressable Failed");
            }
        };
    }

    private void InitializeLoad(Action callback)
    {
        LoadManager.Load(() =>
        {
            callback?.Invoke();
        });
    }

    private void InitializeComplete(Client client)
    {
        client?.RunGame();
    }
}
