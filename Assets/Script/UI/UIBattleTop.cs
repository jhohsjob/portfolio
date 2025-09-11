using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UIBattleTop : MonoBehaviour
{
    [SerializeField]
    private Button _btnPause;

    [SerializeField]
    private TextMeshProUGUI _txtGold;

    private void Awake()
    {
        _txtGold.text = "0";

        _btnPause.onClick.AddListener(OnClickPause);

        EventHelper.AddEventListener(EventName.AddGold, OnAddGold);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.AddGold, OnAddGold);
    }

    private void OnClickPause()
    {
        BattleManager.instance.SetBattleStatus(BattleStatus.Pause);

        UIManager.instance.ShowPopup(PopupName.UIPause);
    }


    private void OnAddGold(object sender, object data)
    {
        _txtGold.text = (string)data;
    }
}
