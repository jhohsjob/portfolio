using System;
using UnityEngine;


public class UIPausePopupData
{
    public Action onResumeBattle { get; }
    public ISceneService sceneLoader { get; }

    public UIPausePopupData(Action onResumeBattle, ISceneService sceneLoader)
    {
        this.onResumeBattle = onResumeBattle;
        this.sceneLoader = sceneLoader;
    }
}

public class UIPausePopupPresenter : IDisposable
{
    private readonly UIPausePopup _view;
    private readonly UIPausePopupData _data;

    public UIPausePopupPresenter(UIPausePopup view, UIPausePopupData data)
    {
        _view = view;
        _data = data;

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

        _data.sceneLoader.LoadLobbyScene();
    }

    private void OnClickResume()
    {
        _data.onResumeBattle?.Invoke();
    }
}