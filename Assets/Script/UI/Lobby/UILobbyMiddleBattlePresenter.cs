using System.Collections.Generic;


public class UILobbyMiddleBattlePresenter : UILobbyMiddlePresenter<UILobbyMiddleBattle>
{
    private List<Stage> _stages;
    private List<Mercenary> _mercenaries;

    public UILobbyMiddleBattlePresenter(UILobbyMiddleBattle view, UILobbyContext context) : base(view, context)
    {
        _mercenaries = new List<Mercenary>(MercenaryManager.instance.list);

        _stages = new List<Stage>(_context.stageService.list);
        _stages.Reverse();
    }

    protected override void Bind()
    {
        _view.onShowPanel += OnShowPanel;
        _view.onStageClick += OnStageClick;

        _view.onGetStageCount = () => _stages.Count;
        _view.onGetStageItemData = GetStageItemData;

        _view.onGetMercenaryCount = () => _mercenaries.Count;
        _view.onGetMercenaryData = (index) => _mercenaries[index];
    }

    public override void Initialize()
    {
        int stageListIndex = _context.stageService.GetStageIndexById(_context.user.currentStageId);
        _context.assetLoader.LoadPrefab("BattleStageItem", prefab =>
        {
            _view.SetupStageScroll(prefab, stageListIndex);
        });

        int mercenaryListIndex = _mercenaries.FindIndex(x => x.id == _context.user.mercenaryId);
        _context.assetLoader.LoadPrefab("BattleMercenaryItem", prefab =>
        {
            _view.SetupMercenaryScroll(prefab, mercenaryListIndex);
        });
    }
    
    protected override void Unbind()
    {
        _view.onShowPanel -= OnShowPanel;
        _view.onStageClick -= OnStageClick;

        _view.onGetStageCount = null;
        _view.onGetStageItemData = null;
        _view.onGetMercenaryCount = null;
        _view.onGetMercenaryData = null;
    }

    private void OnShowPanel()
    {
        _view.RefreshMercenaryScroll();
    }

    private UIBattleStageScrollItemData GetStageItemData(int index)
    {
        if (index < 0 || index >= _stages.Count)
        {
            return null;
        }

        return new UIBattleStageScrollItemData
        {
            stage = _stages[index],
            state = _context.stageService.IsStageStateByIndex(index, _context.user.currentStageId)
        };
    }

    private void OnStageClick(Stage stage)
    {
        int mercenaryIndex = _view.GetCenteredMercenaryIndex();
        if (mercenaryIndex < 0 || mercenaryIndex >= _mercenaries.Count)
        {
            return;
        }

        var mercenary = _mercenaries[mercenaryIndex];

        if (mercenary.isOwned == false)
        {
            var popupData = new UICommonPopupData
            {
                title = "알림",
                message = "용병이 잠겨 있습니다", // todo : locale
            };
            _context.popupService.ShowPopup<UICommonPopup>(PopupName.UICommonPopup, popupData);
            return;
        }

        _context.user.SetMercenary(mercenary.id);

        GameSession.instance.SetMercenary(mercenary);
        GameSession.instance.SetStage(stage);

        _context.sceneLoader.LoadBattleScene();
    }
}