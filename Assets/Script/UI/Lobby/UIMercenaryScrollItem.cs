using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIMercenaryScrollItem : MonoBehaviour
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

    public void Init(Mercenary mercenary)
    {
        _data = mercenary;

        _name.text = mercenary.name;
        _icon.sprite = mercenary.icon;
    }

    public void OnClickItem()
    {
        PopupManager.ShowPopup<UISelectMercenary>(PopupName.UISelectMercenary, _data);
    }
}
