using System;
using System.Collections.Generic;
using UnityEngine;


public class UILobbyMiddleBattle : UILobbyMiddle
{
    private class StageScrollProvider : IScrollDataProvider
    {
        private readonly List<Stage> _stages;
        private readonly Action<Stage> _onClick;

        public StageScrollProvider(List<Stage> stages, Action<Stage> onClick)
        {
            _stages = stages;
            _onClick = onClick;
        }

        public int GetItemCount()
        {
            return _stages.Count;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            var stageItem = item as UIBattleStageScrollItem;
            stageItem.SetData(index, _stages[index]);
            stageItem.SetOnClick(_onClick);
        }
    }

    private class MercenaryScrollProvider : IScrollDataProvider
    {
        private readonly List<Mercenary> _mercenaries;
        private readonly HorizontalInfiniteScroll _scroll;

        public MercenaryScrollProvider(List<Mercenary> mercenaries, HorizontalInfiniteScroll scroll)
        {
            _mercenaries = mercenaries;
            _scroll = scroll;
        }

        public int GetItemCount()
        {
            return _mercenaries.Count;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            item.SetData(index, _mercenaries[index]);

            float scale = _scroll.CalculateScaleForItem(item);
            item.ForceSetScale(scale);
        }
    }

    [SerializeField]
    private VerticalInfiniteScroll _stageScroll;
    [SerializeField]
    private HorizontalInfiniteScroll _mercenaryScroll;

    private List<Stage> _stages;
    private List<Mercenary> _mercenaries;

    protected override void Awake()
    {
        base.Awake();

        _stages = new List<Stage>(StageManager.instance.list);
        _stages.Reverse(); // todo : 클리어 스테이지, 현재 스테이지보다 높은 단계의 스테이지 제거하여 리스트 크기 줄이기
        int listIndex = StageManager.instance.GetStageIndexById(Client.user.currentStageId);

        Client.asset.LoadAsset<GameObject>("BattleStageItem", (task) =>
        {
            var prefab = task.GetAsset<GameObject>();

            _stageScroll.Initialize(
                provider: new StageScrollProvider(_stages, StartStage),
                factory: new StageItemFactory(prefab),
                itemCount: _stages.Count,
                initPos: listIndex
            );
        });

        _mercenaries = new List<Mercenary>(MercenaryManager.instance.list);

        Client.asset.LoadAsset<GameObject>("BattleMercenaryItem", task =>
        {
            var prefab = task.GetAsset<GameObject>();

            _mercenaryScroll.Initialize(
                provider: new MercenaryScrollProvider(_mercenaries, _mercenaryScroll),
                factory: new MercenaryItemFactory(prefab),
                itemCount: _mercenaries.Count,
                initPos: _mercenaries.FindIndex(x => x.id == Client.user.mercenaryId)
            );
        });
    }

    protected override void OnShow()
    {
        base.OnShow();

        _mercenaryScroll.UpdateItems();
    }

    private void StartStage(Stage stage)
    {
        int mercenaryIndex = _mercenaryScroll.GetCenteredIndex();
        var mercenary = _mercenaries[mercenaryIndex];

        if (mercenary.isOwned == false)
        {
            return;
        }

        GameSession.instance.SetMercenary(mercenary);
        GameSession.instance.SetStage(stage);

        SceneLoader.LoadBattleScene();
    }
}