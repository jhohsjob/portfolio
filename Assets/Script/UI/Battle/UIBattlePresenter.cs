using System;
using UnityEngine;


public class UIBattlePresenter : IDisposable
{
    public sealed class Deps
    {
        public IAssetService assetService;
        public ISceneService sceneLoader;
        public IPopupService popupService;
        public IBattleController battleController;
        public BattleRewardRuntime battleReward;
        public DashController dashController;
        public Action onDashAction;
        public Action<Vector2> onJoystickAction;
        public Camera mainCamera;
    }

    
    private readonly UIBattle _view;
    private readonly Deps _deps;

    private UIBattleTopPresenter _topPresenter;
    private UIDashPresenter _dashPresenter;
    private HPBarManager _hpBarManager;

    public UIBattlePresenter(UIBattle view, Deps deps)
    {
        _view = view;
        _deps = deps;

        _topPresenter = new UIBattleTopPresenter(_view.TopView, new UIBattleTopPresenter.Deps
        {
            popupService = deps.popupService,
            sceneLoader = deps.sceneLoader,
            battleController = deps.battleController,
            battleReward = deps.battleReward
        });

        _dashPresenter = new UIDashPresenter(_view.DashView, deps.dashController, deps.onDashAction);

        _hpBarManager = view.hpBarManager;
        _hpBarManager.InitDependencies(deps.mainCamera, deps.assetService);

        Bind();

        _view.PanelChange(UIBattle.PanelMode.Start);
    }

    public void Dispose()
    {
        _topPresenter?.Dispose();
        _dashPresenter?.Dispose();

        Unbind();
    }

    private void Bind()
    {
        _view.onClickLobby += OnClickLobby;
        _view.JoystickView.onJoystickMove += _deps.onJoystickAction;

        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);
    }

    private void Unbind()
    {
        _view.onClickLobby -= OnClickLobby;
        _view.JoystickView.onJoystickMove -= _deps.onJoystickAction;

        EventHelper.RemoveEventListener(EventName.BattleStatus, OnBattleStatus);
    }

    public void Initialize()
    {
        _topPresenter.Initialize();
        _dashPresenter.Initialize();
        _hpBarManager.Initialize();
    }

    private void OnClickLobby()
    {
        _deps.sceneLoader.LoadLobbyScene();
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
                _view.StartDirectionAsync().Forget();
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