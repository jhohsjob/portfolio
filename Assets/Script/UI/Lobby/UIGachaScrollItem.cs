using UnityEngine;
using UnityEngine.UI;


public class UIGachaScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;

    private GameObject _scrollUI;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        if (data is not Gacha gacha || gacha.scrollUI == null)
        {
            return;
        }
    }

    public void SetUI(GameObject scroll)
    {
        if (scroll == null)
        {
            return;
        }

        if (scroll.transform.parent != _root)
        {
            scroll.transform.SetParent(_root, false);
        }

        if (_scrollUI != null)
        {
            _scrollUI.SetActive(false);
        }

        _scrollUI = scroll;
        _scrollUI.SetActive(true);
    }

    public void OnClickItem()
    {
        _scroll.MoveToIndex(index);
    }
}