using System.Collections.Generic;


public class UIBattleTopContext
{
    public IPopupService popupService;
    public ISceneLoader sceneLoader;
    public IBattleController battleController;
    public BattleRewardRuntime battleReward;
}

public class UIBattleTopPresenter
{
    private readonly UIBattleTop _view;
    private readonly UIBattleTopContext _context;

    public UIBattleTopPresenter(UIBattleTop view, UIBattleTopContext context)
    {
        _view = view;
        _context = context;

        Bind();
    }

    private void Bind()
    {
        _view.onClickPause += OnClickPause;

        _context.battleReward.onGoldChanged += OnGoldChanged;

        EventHelper.AddEventListener(EventName.AddElement, OnAddElement);
    }

    public void Initialize()
    {
        _view.SetGold(0);
    }

    public void Dispose()
    {
        _view.onClickPause -= OnClickPause;

        _context.battleReward.onGoldChanged -= OnGoldChanged;

        EventHelper.RemoveEventListener(EventName.AddElement, OnAddElement);
    }

    private void OnClickPause()
    {
        _context.battleController.SetStatus(BattleStatus.Paused);

        var popupData = new UIPausePopupContext(OnBattleResume, _context.sceneLoader);
        _context.popupService.ShowPopup<UIPausePopup>(PopupName.UIPausePopup, popupData, popup =>
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
        _context.battleController.SetStatus(BattleStatus.Running);
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