using System;
using UnityEngine;


public class UILobbyMiddleBarrack : UILobbyMiddleBase
{
    private class BarrackScrollProvider : IScrollDataProvider
    {
        private readonly UILobbyMiddleBarrack _view;

        public BarrackScrollProvider(UILobbyMiddleBarrack view)
        {
            _view = view;
        }

        public int GetItemCount()
        {
            return _view.onGetItemCount?.Invoke() ?? 0;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            if (item is not UIBarrackMercenaryScrollItem mercenaryItem)
            {
                return;
            }
            var mercenary = _view.onGetMercenary?.Invoke(index);
            if (mercenary == null)
            {
                return;
            }

            mercenaryItem.SetData(index, mercenary);
            mercenaryItem.SetOnClick(mercenary => _view.onClickItem?.Invoke(mercenary));
        }
    }

    [SerializeField]
    private VerticalInfiniteScroll _scroll;

    public event Action onClickShow;
    public Func<int, Mercenary> onGetMercenary;
    public Func<int> onGetItemCount;
    public Action<Mercenary> onClickItem;

    public void SetupScroll(GameObject prefab)
    {
        _scroll.Initialize(
            provider: new BarrackScrollProvider(this),
            factory: new ScrollItemFactory(prefab)
        );
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
}