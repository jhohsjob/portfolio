using System;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyTop : MonoBehaviour
{
    [SerializeField]
    private UIGold _uiGold;
    [SerializeField]
    private Button _btnSetting;

    public event Action onClickSetting;
    public Action onDestroyAction;

    public UIGold goldView => _uiGold;

    private void Awake()
    {
        _btnSetting.onClick.AddListener(() => onClickSetting?.Invoke());
    }

    private void OnDestroy()
    {
        onDestroyAction?.Invoke();
        onDestroyAction = null;
    }
}