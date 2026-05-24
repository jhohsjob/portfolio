using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UIBarrackMercenaryScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Image _icon;

    private Mercenary _data;
    private Action<Mercenary> _onClick;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= locale => { UpdateUI(); };
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _data = (Mercenary)data;

        UpdateUI();

        LocalizationSettings.SelectedLocaleChanged += locale => { UpdateUI(); };
    }

    private void UpdateUI()
    {
        _name.text = _data.name;
        _icon.sprite = _data.icon;
        _icon.color = _data.isOwned ? Color.wheat : Color.black;
        // _lock.gameObject.SetActive(!_data.isOwned);
    }

    public void SetOnClick(Action<Mercenary> onClick)
    {
        _onClick = onClick;
    }

    public void OnClickItem()
    {
        _onClick?.Invoke(_data);
    }
}
