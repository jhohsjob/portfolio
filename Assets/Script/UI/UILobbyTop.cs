using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyTop : MonoBehaviour
{
    [SerializeField]
    private Button _btnSetting;
    
    [SerializeField]
    private TextMeshProUGUI _txtGold;

    private void Awake()
    {
        OnChangeGold(null, null);

        EventHelper.AddEventListener(EventName.ChangeGold, OnChangeGold);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ChangeGold, OnChangeGold);
    }

    private void OnChangeGold(object sender, object data)
    {
        _txtGold.text = User.instance.gold.ToString();
    }
}
