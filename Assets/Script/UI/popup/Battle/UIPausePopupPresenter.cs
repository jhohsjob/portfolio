using System;
using UnityEngine;


public class UIPausePopupContext
{
    public Action onResumeBattle { get; }
    public ISceneLoader sceneLoader { get; }

    public UIPausePopupContext(Action onResumeBattle, ISceneLoader sceneLoader)
    {
        this.onResumeBattle = onResumeBattle;
        this.sceneLoader = sceneLoader;
    }
}

public class UIPausePopupPresenter : IDisposable
{
    private readonly UIPausePopup _view;
    private readonly UIPausePopupContext _context;

    public UIPausePopupPresenter(UIPausePopup view, UIPausePopupContext context)
    {
        _view = view;
        _context = context;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        _view.onClickLobby += OnClickLobby;
        _view.onClickResume += OnClickResume;
        // _view.onClickOption += OnClickOption;
    }

    private void Unbind()
    {
        _view.onClickLobby -= OnClickLobby;
        _view.onClickResume -= OnClickResume;
    }

    private void OnClickLobby()
    {
        Time.timeScale = 1f;

        _context.sceneLoader.LoadLobbyScene();
    }

    private void OnClickResume()
    {
        _context.onResumeBattle?.Invoke();
    }
}