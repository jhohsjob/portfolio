using System;
using UnityEngine;
using UnityEngine.UI;


public class UIBarrackMercenaryScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private UIMercIcon _mercIcon;

    private Mercenary _mercenary;
    private Action<Mercenary> _onClick;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _mercenary = (Mercenary)data;

        _mercIcon.Initialize(_mercenary);
    }

    public void SetOnClick(Action<Mercenary> onClick)
    {
        _onClick = onClick;
    }

    public void OnClickItem()
    {
        _onClick?.Invoke(_mercenary);
    }
}
