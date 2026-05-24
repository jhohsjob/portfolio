using System;
using UnityEngine;


public class UILobby : MonoBehaviour
{
    [SerializeField]
    private UILobbyTop _top;
    [SerializeField]
    private RectTransform _middle;
    [SerializeField]
    private UIHorizontalMenuFitter _bottom;

    private UILobbyMiddleBase[] _middles;
    public int menuCount => _bottom.menuCount;
    public event Action<int> onClickBottomMenu;

    public UILobbyTop TopView =>_top;
    public UILobbyMiddleBase[] MiddleViews => _middles;

    private void Awake()
    {
        _middles = _middle.GetComponentsInChildren<UILobbyMiddleBase>();

        _bottom.onMenuChange += index =>
        {
            onClickBottomMenu?.Invoke(index);
        };
    }

    public int GetMiddleCount()
    {
        return _middles.Length;
    }

    public void HideAllMiddle()
    {
        foreach (var middle in _middles)
        {
            middle.Hide();
        }
    }

    public void ShowMiddle(int index)
    {
        _middles[index].Show();
    }
}
