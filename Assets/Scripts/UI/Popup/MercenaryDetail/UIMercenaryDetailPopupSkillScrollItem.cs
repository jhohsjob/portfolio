using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UIMercenaryDetailPopupSkillScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private GameObject _arrow;

    private SkillData _data;
    private Action<SkillData, Transform> _onClick;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);

        LocalizationSettings.SelectedLocaleChanged += HandleLocaleChanged;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= HandleLocaleChanged;
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        if (data is not SkillData skillData)
        {
            return;
        }
        _data = skillData;

        UpdateUI();
    }

    private void UpdateUI()
    {
        _name.text = _data.name;
        _icon.sprite = _data.icon;

        _arrow.SetActive(_data.lastData == false);
    }

    public void SetOnClick(Action<SkillData, Transform> onClick)
    {
        _onClick = onClick;
    }

    public void OnClickItem()
    {
        _onClick?.Invoke(_data, transform);
    }

    private void HandleLocaleChanged(Locale locale)
    {
        UpdateUI();
    }
}
