using System;
using UnityEngine;
using UnityEngine.UI;


public class UIPausePopup : UIPopup
{
    [SerializeField]
    protected Button _btnOption;
    [SerializeField]
    protected Button _btnLobby;

    public event Action onClickOption;
    public event Action onClickLobby;
    public event Action onClickResume;

    protected override void Awake()
    {
        base.Awake();

        _btnOption.onClick.AddListener(() => onClickOption?.Invoke());
        _btnLobby.onClick.AddListener(() => onClickLobby?.Invoke());
    }

    protected override void OnClickClose()
    {
        onClickResume?.Invoke();

        base.OnClickClose();
    }
}