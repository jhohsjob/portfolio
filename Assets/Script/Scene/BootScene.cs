using UnityEngine;


public class BootScene : MonoBehaviour
{
    [SerializeField]
    private Launch _launch;
    [SerializeField]
    private UIBoot _uiBoot;

    private BootController _bootController;
    private BootPresenter _bootPresenter;

    private void Awake()
    {
        _bootController = new BootController();
        _bootPresenter = new BootPresenter(_uiBoot, _bootController, _launch.Context.SceneService);
    }

    private async void Start()
    {
        try
        {
            await _launch.RunAsync(_bootController);
            OnBootFinished();
        }
        catch
        {
            // 예외 처리
        }
    }

    private void Update()
    {
        _bootController?.Update(Time.deltaTime);
    }

    private void OnBootFinished()
    {
        _bootController?.Complete();
    }

    private void OnDestroy()
    {
        _bootPresenter?.Dispose();
    }
}