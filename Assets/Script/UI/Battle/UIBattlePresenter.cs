using System;
using UnityEngine;


public class UIBattleContext
{
    public IAssetLoader assetLoader;
    public IPopupService popupService;
    public ISceneLoader sceneLoader;
    public IBattleController battleController;
    public BattleRewardRuntime battleReward;
    public DashController dashController;
    public Action onDashAction;
    public Action<Vector2> onJoystickAction;
}

public class UIBattlePresenter : IDisposable
{
    private readonly UIBattle _view;
    private readonly UIBattleContext _context;

    private UIBattleTopPresenter _topPresenter;
    private UIDashPresenter _dashPresenter;
    private HPBarController _hpBarController;

    public UIBattlePresenter(UIBattle view, UIBattleContext context)
    {
        _view = view;
        _context = context;

        _topPresenter = new UIBattleTopPresenter(_view.TopView, new UIBattleTopContext
        {
            popupService = context.popupService,
            sceneLoader = context.sceneLoader,
            battleController = context.battleController,
            battleReward = context.battleReward
        });

        _dashPresenter = new UIDashPresenter(_view.DashView, context.dashController, context.onDashAction);

        _hpBarController = view.hpBarController;
        _hpBarController.InitDependencies(context.assetLoader);

        Bind();
    }

    private void Bind()
    {
        _view.onClickLobby += OnClickLobby;
        _view.JoystickView.onJoystickMove += _context.onJoystickAction;

        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);

        _view.PanelChange(UIBattle.PanelMode.Start);
    }

    public void Initialize()
    {
        _topPresenter.Initialize();
        _dashPresenter.Initialize();
        _hpBarController.Initialize();
    }

    public void Dispose()
    {
        _topPresenter?.Dispose();
        _dashPresenter?.Dispose();

        _view.onClickLobby -= OnClickLobby;
        _view.JoystickView.onJoystickMove -= _context.onJoystickAction;

        EventHelper.RemoveEventListener(EventName.BattleStatus, OnBattleStatus);
    }

    private void OnClickLobby()
    {
        _context.sceneLoader.LoadLobbyScene();
    }

    private void OnBattleStatus(object sender, object data)
    {
        if (data is not BattleStatus status)
        {
            return;
        }

        switch (status)
        {
            case BattleStatus.Running:
                IEnumeratorTool.instance.StartCoroutine(_view.StartDirection());
                break;

            case BattleStatus.Win:
                _view.BattleWinDirection();
                break;

            case BattleStatus.Lose:
                _view.BattleLoseDirection();
                break;
        }
    }
}