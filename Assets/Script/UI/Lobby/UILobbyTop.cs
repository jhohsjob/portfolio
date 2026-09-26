using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyTop : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtLevel;
    [SerializeField]
    private UIGold _uiGold;
    [SerializeField]
    private Button _btnSetting;

    public event Action onClickSetting;

    private void Awake()
    {
        _btnSetting.onClick.AddListener(HandleClickSetting);
    }

    public void SetLevelText(int level)
    {
        _txtLevel.text = $"{level}";
    }

    public void SetGoldText(int gold)
    {
        _uiGold.SetGoldText(gold);
    }

    private void HandleClickSetting()
    {
        onClickSetting?.Invoke();
    }
}