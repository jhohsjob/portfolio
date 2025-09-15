using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class Launch : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        InitializeAddressable();
    }

    private void InitializeAddressable()
    {
        Addressables.InitializeAsync().Completed += (data) =>
        {
            Debug.Log("InitializeAddressable Status : " + data.Status + ", OperationException :  " + data.OperationException);

            if (data.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("InitializeAddressable Succeeded LocatorId : " + data.Result.LocatorId);

                InitializeComplete();
            }
            else
            {
                Debug.Log("InitializeAddressable Failed");
            }
        };
    }

    private void InitializeComplete()
    {
        new Client().RunGame();
    }
}
