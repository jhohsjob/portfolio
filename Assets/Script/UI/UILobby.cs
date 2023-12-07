using UnityEngine;
using UnityEngine.UI;


public class UILobby : MonoBehaviour
{
    [SerializeField]
    private Button _btnBattleStart;
    [SerializeField]
    private Button _btnExit;

    private void Awake()
    {
        _btnBattleStart.onClick.AddListener(OnClickStart);
        _btnExit.onClick.AddListener(OnClickExit);
    }

    private void OnClickStart()
    {
        UIManager.instance.ShowPopup(PopupName.UISelectMap);
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
