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

        public void Bind(int index, InfiniteScrollItem scrollItem)
        {
            if (scrollItem is not UIBattleStageScrollItem item)
            {
                return;
            }

            var itemData = _view.onGetStageData?.Invoke(index);
            if (itemData == null)
            {
                return;
            }

            item.SetData(index, itemData);
            item.SetOnClick(stage => _view.onClikcStageItem?.Invoke(stage));
        }
    }

    private class MercenaryScrollProvider : IScrollDataProvider
    {
        private readonly UILobbyMiddleBattle _view;

        public MercenaryScrollProvider(UILobbyMiddleBattle view)
        {
            _view = view;
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
        }
    }

    [SerializeField]
    private VerticalInfiniteScroll _stageScroll;
    [SerializeField]
    private HorizontalInfiniteScroll _mercenaryScroll;

    public event Action<Stage, int> onStartStageRequest;

    public Func<int> onGetStageCount;
    public Func<int, UIBattleStageScrollItemData> onGetStageData;
    public Action<Stage> onClikcStageItem;

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
            provider: new MercenaryScrollProvider(this),
            factory: new MercenaryItemFactory(prefab),
            itemCount: onGetMercenaryCount?.Invoke() ?? 0,
            initPos: initIndex
        );
        _mercenaryScroll.UpdateItems();
    }

    public int GetCenteredMercenaryIndex()
    {
        return _mercenaryScroll.GetCenteredIndex();
    }
}