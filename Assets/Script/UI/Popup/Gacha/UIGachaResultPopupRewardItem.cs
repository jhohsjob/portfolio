using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIGachaResultPopupRewardItem : InfiniteScrollItem
{
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private Image _icon;

    private MercenaryDefinition _mercenary;

    private void Awake()
    {
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _mercenary = (MercenaryDefinition)data;

        _txtName.text = LocalizationHelper.GetMercenaryString(_mercenary.GetNameKey());
        _icon.sprite = _mercenary.icon;
    }
}
