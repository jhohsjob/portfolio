using TMPro;
using UnityEngine;


public class UIGold : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _txtGold;

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.ChangeGold, OnChangeGold);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ChangeGold, OnChangeGold);
    }

    private void OnChangeGold(object sender, object data)
    {
        if (Client.user == null)
        {
            return;
        }

        _txtGold.text = Client.user.gold.ToString();
    }
}
