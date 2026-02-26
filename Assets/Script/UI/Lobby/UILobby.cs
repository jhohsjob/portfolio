using UnityEngine;


public class UILobby : MonoBehaviour
{
    [SerializeField]
    private UIHorizontalMenuFitter _bottom;
    [SerializeField]
    private RectTransform _middle;

    private UILobbyMiddle[] _middles;
    private int _menuCount;

    private void Awake()
    {
        _middles = _middle.GetComponentsInChildren<UILobbyMiddle>();

        _bottom.cbInitialized += (count) => { _menuCount = count; };
        _bottom.cbMenuChange += OnClickBottomMenu;
    }

    private void OnClickBottomMenu(int index)
    {
        if (index >= _menuCount || index >= _middles.Length)
        {
            Debug.Log("menu miss match");
            return;
        }

        foreach (var middle in _middles)
        {
            middle.Hide();
        }

        _middles[index].Show();
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
