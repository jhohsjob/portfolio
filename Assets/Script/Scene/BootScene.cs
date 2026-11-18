using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;


public class BootScene : MonoBehaviour
{
    [SerializeField]
    private Launch _launch;
    [SerializeField]
    private GlobalRoot _global;
    [SerializeField]
    private UIBoot _uiBoot;

    private BootController _bootController = new();
    private BootPresenter _bootPresenter;

    private CancellationTokenSource _cts;

    private void Awake()
    {
        Debug.Log("BootScene Awake");

        var app = App.EnsureCreated(_global);
        _launch.Begin(app);
    }

    private async void Start()
    {
        Debug.Log("BootScene Start");

        _bootPresenter = new BootPresenter(_uiBoot, _bootController, _launch.sceneService);
        
        await BootAsync();
    }

    private void Update()
    {
        _bootController?.Update(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _bootPresenter?.Dispose();
    }

    private async UniTask BootAsync()
    {
        _cts = new CancellationTokenSource();

        try
        {
            await _launch.RunAsync(_bootController, _cts.Token);
            _bootController.Complete();
        }
        catch (OperationCanceledException) { }
        catch (Exception e)
        {
            Debug.LogException(e);
            _bootController.Fail(e);
        }
    }
}