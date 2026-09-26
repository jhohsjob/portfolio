using System.Collections.Generic;


public class UILobbyMiddleBattlePresenter : UILobbyMiddlePresenter<UILobbyMiddleBattle>
{
    private List<Stage> _stages;
    private List<Mercenary> _mercenaries;

    public UILobbyMiddleBattlePresenter(UILobbyMiddleBattle view, UILobbyContext context) : base(view, context)
    {
        _mercenaries = new List<Mercenary>(context.mercenaryService.list);

        _stages = new List<Stage>(_context.stageService.list);
        _stages.Reverse();
    }

    protected override void Bind()
    {
        _view.onClikcStageItem += OnClickStageItem;

        _view.onGetStageCount = () => _stages.Count;
        _view.onGetStageData = GetStageData;

        _view.onGetMercenaryCount = () => _mercenaries.Count;
        _view.onGetMercenaryData = (index) => _mercenaries[index];
    }

    protected override void Unbind()
    {
        _view.onClikcStageItem -= OnClickStageItem;

        _view.onGetStageCount = null;
        _view.onGetStageData = null;

        _view.onGetMercenaryCount = null;
        _view.onGetMercenaryData = null;
    }

    public override void Initialize()
    {
        int stageListIndex = _context.stageService.GetStageIndexById(_context.user.lastPlayStageId);
        _context.assetLoader.LoadPrefab("BattleStageItem", prefab =>
        {
            _view.SetupStageScroll(prefab, stageListIndex);
        });

        int mercenaryListIndex = _mercenaries.FindIndex(x => x.id == _context.user.lastPlayMercenaryId);
        _context.assetLoader.LoadPrefab("BattleMercenaryItem", prefab =>
        {
            _view.SetupMercenaryScroll(prefab, mercenaryListIndex);
        });
    }
    
    private UIBattleStageScrollItemData GetStageData(int index)
    {
        if (index < 0 || index >= _stages.Count)
        {
            return null;
        }

        return new UIBattleStageScrollItemData
        {
            stage = _stages[index],
            state = _context.stageService.IsStageStateByIndex(index, _context.user.lastPlayStageId)
        };
    }

    private void OnClickStageItem(Stage stage)
    {
        int mercenaryIndex = _view.GetCenteredMercenaryIndex();
        if (mercenaryIndex < 0 || mercenaryIndex >= _mercenaries.Count)
        {
            return;
        }

        var mercenary = _mercenaries[mercenaryIndex];

        if (mercenary.isOwned == false)
        {
            _context.popupService.ShowCommonPopup("알림", "용병이 잠겨 있습니다");
            return;
        }

        _context.user.SetMercenary(mercenary.id);

        GameSession.instance.SetMercenary(mercenary);
        GameSession.instance.SetStage(stage);

        _context.sceneLoader.LoadBattleScene();
    }
}