using System.Collections.Generic;


public class UILobbyMiddleBattlePresenter : UILobbyMiddlePresenter<UILobbyMiddleBattle>
{
    private List<Stage> _stages;
    private List<Mercenary> _mercenaries;

    public UILobbyMiddleBattlePresenter(UILobbyMiddleBattle view, UILobbyPresenter.Deps deps) : base(view, deps)
    {
        _mercenaries = new List<Mercenary>(deps.mercenaryService.list);

        _stages = new List<Stage>(_deps.stageService.list);
        _stages.Reverse();
    }

    protected override void Bind()
    {
        _view.onClickStageItem += OnClickStageItem;

        _view.onGetStageCount = () => _stages.Count;
        _view.onGetStageData = GetStageData;

        _view.onGetMercenaryCount = () => _mercenaries.Count;
        _view.onGetMercenaryData = (index) => _mercenaries[index];
    }

    protected override void Unbind()
    {
        _view.onClickStageItem -= OnClickStageItem;

        _view.onGetStageCount = null;
        _view.onGetStageData = null;

        _view.onGetMercenaryCount = null;
        _view.onGetMercenaryData = null;
    }

    public override void Initialize()
    {
        int stageListIndex = _deps.stageService.GetStageIndexById(_deps.user.lastPlayStageId);
        _deps.assetService.LoadPrefab("BattleStageScrollItem", prefab =>
        {
            _view.SetupStageScroll(prefab, stageListIndex);
        });

        int mercenaryListIndex = _mercenaries.FindIndex(x => x.Id == _deps.user.lastPlayMercenaryId);
        _deps.assetService.LoadPrefab("BattleMercenaryScrollItem", prefab =>
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
            state = _deps.stageService.GetStageState(index, _deps.user.lastPlayStageId)
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
            _deps.popupService.ShowCommonPopup(LocalizationHelper.GetCommonString("notice"),
                                                  LocalizationHelper.GetCommonString("mercenary_lock"));
            return;
        }

        _deps.user.SetMercenary(mercenary.Id);

        _deps.sceneService.LoadBattleScene(new BattleRequest(stage, mercenary));
    }
}