using TMPro;
using UnityEngine;
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

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.LocaleChanged, (sender, data) => UpdateUI());
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _data = (Mercenary)data;

        UpdateUI();

        EventHelper.AddEventListener(EventName.LocaleChanged, (sender, data) => UpdateUI());
    }

    private void UpdateUI()
    {
        _name.text = _data.name;
        _icon.sprite = _data.icon;
        _icon.color = _data.isOwned ? Color.wheat : Color.black;
        // _lock.gameObject.SetActive(!_data.isOwned);
    }

    public void OnClickItem()
    {
        PopupManager.ShowPopup<UIMercenaryDetailPopup>(PopupName.UIMercenaryDetail, _data);
    }
}
