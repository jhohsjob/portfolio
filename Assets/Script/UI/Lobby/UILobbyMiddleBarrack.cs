using System;
using UnityEngine;


public class UILobbyMiddleBarrack : UILobbyMiddleBase, IScrollDataProvider
{
    [SerializeField]
    private VerticalInfiniteScroll _scroll;

    public event Action onClickShow;
    public Func<int, Mercenary> onGetMercenary;
    public Func<int> onGetItemCount;
    public Action<Mercenary> onItemClick;

    public void SetupScroll(GameObject prefab)
    {
        var factory = new MercenaryItemFactory(prefab);
        _scroll.Initialize(this, factory, onGetItemCount?.Invoke() ?? 0);
    }

    protected override void OnShow()
    {
        base.OnShow();

        onClickShow?.Invoke();
    }

    public void RefreshScroll()
    {
        _scroll.UpdateItems();
    }

    public int GetItemCount()
    {
        return onGetItemCount?.Invoke() ?? 0;
    }

    public void Bind(int index, InfiniteScrollItem item)
    {
        if (item is not UIBarrackMercenaryScrollItem mercenaryItem)
        {
            return;
        }
        var mercenary = onGetMercenary?.Invoke(index);
        if (mercenary == null)
        {
            return;
        }

        mercenaryItem.SetData(index, mercenary);
        mercenaryItem.SetOnClick(mercenary => onItemClick?.Invoke(mercenary));
    }
}