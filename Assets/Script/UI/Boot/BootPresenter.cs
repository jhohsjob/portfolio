public class BootPresenter
{
    private readonly ISceneLoader _sceneLoader;

    private readonly UIBoot _view;
    private readonly BootController _controller;

    public BootPresenter(UIBoot view, BootController controller, ISceneLoader sceneLoader)
    {
        _view = view;
        _controller = controller;

        _sceneLoader = sceneLoader;

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
        _sceneLoader.LoadLobbyScene();
    }
}