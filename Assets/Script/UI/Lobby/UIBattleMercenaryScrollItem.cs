using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattleMercenaryScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Image _icon;

    private Mercenary _data;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _data = (Mercenary)data;

        _name.text = _data.name;
        _icon.sprite = _data.icon;
    }

    public void OnClickItem()
    {
        _scroll.MoveToIndex(index);
    }
}