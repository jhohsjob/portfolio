using UnityEngine;


public class BootScene : MonoBehaviour
{
    [SerializeField]
    private Launch _launch;
    [SerializeField]
    private UIBoot _uiBoot;

    private BootController _bootController;

    private void Awake()
    {
        _bootController = new BootController();
        _bootController.onProgressChanged += _uiBoot.SetProgress;

        _launch.Initialize(_bootController);
    }

    private async void Start()
    {
        try
        {
            await _launch.RunAsync();
            OnBootFinished();
        }
        catch
        {
            // 예외 처리
        }
    }

    private void Update()
    {
        _bootController.Update(Time.deltaTime);
    }

    private void OnBootFinished()
    {
        _bootController.Complete();
    }
}
