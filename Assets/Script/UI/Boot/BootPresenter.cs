public class BootPresenter
{
    private readonly ISceneService _sceneService;

    private readonly UIBoot _view;
    private readonly BootController _controller;

    public BootPresenter(UIBoot view, BootController controller, ISceneService sceneService)
    {
        _view = view;
        _controller = controller;

        _sceneService = sceneService;

        _controller.onProgressChanged += HandleProgressChanged;

        _view.onNextSceneClicked += HandleNextSceneClicked;

        _view.StartLoadingAnimation();
    }

    public void Dispose()
    {
        _controller.onProgressChanged -= HandleProgressChanged;
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