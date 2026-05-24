using System;
using UnityEngine;


public class UILobbyMiddleBattle : UILobbyMiddleBase
{
    private class StageScrollProvider : IScrollDataProvider
    {
        private readonly UILobbyMiddleBattle _view;

        public StageScrollProvider(UILobbyMiddleBattle view)
        {
            _view = view;
        }

        public int GetItemCount()
        {
            return _view.onGetStageCount?.Invoke() ?? 0;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            if (item is not UIBattleStageScrollItem stageItem)
            {
                return;
            }

            var itemData = _view.onGetStageItemData?.Invoke(index);
            if (itemData == null)
            {
                return;
            }

            stageItem.SetData(index, itemData);
            stageItem.SetOnClick(stage => _view.onStageClick?.Invoke(stage));
        }
    }

    private class MercenaryScrollProvider : IScrollDataProvider
    {
        private readonly UILobbyMiddleBattle _view;
        private readonly HorizontalInfiniteScroll _scroll;

        public MercenaryScrollProvider(UILobbyMiddleBattle view, HorizontalInfiniteScroll scroll)
        {
            _view = view;
            _scroll = scroll;
        }

        public int GetItemCount()
        {
            return _view.onGetMercenaryCount?.Invoke() ?? 0;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            var mercenary = _view.onGetMercenaryData?.Invoke(index);
            if (mercenary == null)
            {
                return;
            }

            item.SetData(index, mercenary);

            float scale = _scroll.CalculateScaleForItem(item);
            item.ForceSetScale(scale);
        }
    }

    [SerializeField]
    private VerticalInfiniteScroll _stageScroll;
    [SerializeField]
    private HorizontalInfiniteScroll _mercenaryScroll;

    public event Action onShowPanel;
    public event Action<Stage, int> onStartStageRequest;

    public Func<int> onGetStageCount;
    public Func<int, UIBattleStageScrollItemData> onGetStageItemData;
    public Action<Stage> onStageClick;

    public Func<int> onGetMercenaryCount;
    public Func<int, Mercenary> onGetMercenaryData;

    public void SetupStageScroll(GameObject prefab, int initIndex)
    {
        _stageScroll.Initialize(
            provider: new StageScrollProvider(this),
            factory: new StageItemFactory(prefab),
            itemCount: onGetStageCount?.Invoke() ?? 0,
            initPos: initIndex
        );
    }

    public void SetupMercenaryScroll(GameObject prefab, int initIndex)
    {
        _mercenaryScroll.Initialize(
            provider: new MercenaryScrollProvider(this, _mercenaryScroll),
            factory: new MercenaryItemFactory(prefab),
            itemCount: onGetMercenaryCount?.Invoke() ?? 0,
            initPos: initIndex
        );
    }

    protected override void OnShow()
    {
        base.OnShow();
        onShowPanel?.Invoke();
    }

    public void RefreshMercenaryScroll()
    {
        _mercenaryScroll.UpdateItems();
    }

    public int GetCenteredMercenaryIndex()
    {
        return _mercenaryScroll.GetCenteredIndex();
    }
}