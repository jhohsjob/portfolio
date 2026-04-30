using UnityEngine;
using UnityEngine.UI;


public class UILobbyTop : MonoBehaviour
{
    [SerializeField]
    private Button _btnSetting;
    
    private void Awake()
    {
        _btnSetting.onClick.AddListener(OnClickSetting);
    }

    private void OnDestroy()
    {
    }

    private void OnClickSetting()
    {
        PopupManager.ShowPopup<UISettingPopup>(PopupName.UISetting);
    }
}
