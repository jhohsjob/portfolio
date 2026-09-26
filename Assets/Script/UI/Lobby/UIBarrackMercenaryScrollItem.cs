using System;
using UnityEngine;
using UnityEngine.UI;


public class UIBarrackMercenaryScrollItem : InfiniteScrollItem
{
    [SerializeField]
    private Button _btn;
    [SerializeField]
    private UIMercIcon _mercIcon;
    [SerializeField]
    private Button _btnBuy;

    private Mercenary _mercenary;
    private Action<Mercenary> _onClick;
    private Action<Mercenary> _onClickBuy;

    private void Awake()
    {
        _btn.onClick.AddListener(OnClickItem);
        _btnBuy.onClick.AddListener(OnClickBuy);
    }

    public override void SetData(int index, object data)
    {
        base.SetData(index, data);

        _mercenary = (Mercenary)data;

        _mercIcon.Initialize(_mercenary);

        _btnBuy.gameObject.SetActive(!_mercenary.isOwned);
    }

    public void SetOnClick(Action<Mercenary> onClick)
    {
        _onClick = onClick;
    }

    public void SetOnClickBuy(Action<Mercenary> onClick)
    {
        _onClickBuy = onClick;
    }

    public void OnClickItem()
    {
        _onClick?.Invoke(_mercenary);
    }

    public void OnClickBuy()
    {
        _onClickBuy?.Invoke(_mercenary);
    }
}
