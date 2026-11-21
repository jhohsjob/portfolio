public class BootPresenter
{
    private readonly ISceneService _sceneService;

    private readonly UIBoot _view;
    private readonly BootProgress _progress;

    public BootPresenter(UIBoot view, BootProgress progress, ISceneService sceneService)
    {
        _view = view;
        _progress = progress;

        _sceneService = sceneService;

        _progress.onProgressChanged += HandleProgressChanged;

        _view.onNextSceneClicked += HandleNextSceneClicked;

        _view.StartLoadingAnimation();
    }

    public void Dispose()
    {
        _progress.onProgressChanged -= HandleProgressChanged;
        _view.onNextSceneClicked -= HandleNextSceneClicked;
    }

    private void HandleProgressChanged(float progress)
    {
        _view.UpdateProgressBar(progress);

        if (progress >= 1f)
        {
            _view.TransitionToTouchToStart();
        }
    }

    private void HandleNextSceneClicked()
    {
        _view.DisableInteraction();
        _sceneService.LoadLobbyScene();
    }
}