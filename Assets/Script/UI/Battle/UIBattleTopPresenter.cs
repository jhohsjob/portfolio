using System.Collections.Generic;


public class UIBattleTopPresenter
{
    public class Deps
    {
        public IPopupService popupService;
        public ISceneService sceneLoader;
        public IBattleController battleController;
        public BattleRewardRuntime battleReward;
    }


    private readonly UIBattleTop _view;
    private readonly Deps _deps;

    public UIBattleTopPresenter(UIBattleTop view, Deps deps)
    {
        _view = view;
        _deps = deps;

        Bind();
    }

    public void Dispose()
    {
        Unbind();
    }

    private void Bind()
    {
        _view.onClickPause += OnClickPause;

        _deps.battleReward.onGoldChanged += OnGoldChanged;

        EventHelper.AddEventListener(EventName.AddElement, OnAddElement);
    }

    private void Unbind()
    {
        _view.onClickPause -= OnClickPause;

        _deps.battleReward.onGoldChanged -= OnGoldChanged;
        
        EventHelper.RemoveEventListener(EventName.AddElement, OnAddElement);
    }

    public void Initialize()
    {
        _view.SetGold(0);
    }

    private void OnClickPause()
    {
        _deps.battleController.SetStatus(BattleStatus.Paused);

        var popupData = new UIPausePopupData(OnBattleResume, _deps.sceneLoader);
        _deps.popupService.ShowPopup<UIPausePopup>(
            address: PopupName.UIPausePopup,
            data: popupData,
            onLoadedCallback: popup =>
            {
                var presenter = new UIPausePopupPresenter(popup, popupData);
                popup.onDestroyAction = () =>
                {
                    presenter?.Dispose();
                    presenter = null;
                };
            });
    }

    private void OnBattleResume()
    {
        _deps.battleController.SetStatus(BattleStatus.Running);
    }

    private void OnGoldChanged(int gold)
    {
        _view.SetGold(gold);
    }

    private void OnAddElement(object sender, object data)
    {
        if (data is not Dictionary<ElementType, int> elements)
        {
            return;
        }

        foreach (var element in elements)
        {
            _view.SetElement(element.Key, element.Value);
        }
    }
}