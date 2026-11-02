using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class UIMercIcon : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtLevel;
    [SerializeField]
    private TextMeshProUGUI _txtGrade;
    [SerializeField]
    private Image _icon;

    private Mercenary _mercenary;

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= locale => { UpdateUI(); };
    }

    public void Initialize(Mercenary mercenary)
    {
        _mercenary = mercenary;

        UpdateUI();

        LocalizationSettings.SelectedLocaleChanged += locale => { UpdateUI(); };
    }

    private void UpdateUI()
    {
        _txtName.text = _mercenary.name;

        _txtLevel.text = _mercenary.isOwned ? $"L.{_mercenary.level}" : string.Empty;
        _txtGrade.text = _mercenary.isOwned ? $"G.{_mercenary.grade}" : string.Empty;
        _icon.sprite = _mercenary.icon;
        _icon.color = _mercenary.isOwned ? Color.wheat : Color.black;
        // _lock.gameObject.SetActive(!_data.isOwned);
    }
}