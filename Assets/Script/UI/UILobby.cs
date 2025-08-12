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
    
    private void Awake()
    {
        _btnStart.onClick.AddListener(OnClickStart);
        _btnSelectMercenary.onClick.AddListener(OnSelectMercenary);
        _btnExit.onClick.AddListener(OnClickExit);
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
}
