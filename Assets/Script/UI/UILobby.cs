using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UILobby : MonoBehaviour
{
    [SerializeField]
    private Button _btnStart;
    [SerializeField]
    private Button _btnSelectMercenary;
    [SerializeField]
    private Button _btnExit;

    [SerializeField]
    private TextMeshProUGUI _txtGold;

    private void Awake()
    {
        OnChangeGold(null, null);

        _btnStart.onClick.AddListener(OnClickStart);
        _btnSelectMercenary.onClick.AddListener(OnSelectMercenary);
        _btnExit.onClick.AddListener(OnClickExit);

        EventHelper.AddEventListener(EventName.ChangeGold, OnChangeGold);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.ChangeGold, OnChangeGold);
    }

    private void OnClickStart()
    {
        UIManager.instance.ShowPopup(PopupName.UISelectMap);
    }

    private void OnSelectMercenary()
    {
        UIManager.instance.ShowPopup(PopupName.UISelectMercenary);
    }

    private void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    private void OnChangeGold(object sender, object data)
    {
        _txtGold.text = $"Gold : {User.instance.gold}";
    }
}
