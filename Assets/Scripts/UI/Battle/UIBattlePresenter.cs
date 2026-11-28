using System;
using UnityEngine;


public class UIBattlePresenter : IDisposable
{
    public sealed class Deps
    {
        public IAssetService assetService;
        public ISceneService sceneLoader;
        public IPopupService popupService;
        public IBattleState battleState;
        public BattleRewardRuntime battleReward;
        public DashAbility dash;
        public Action onDashAction;
        public Action<Vector2> onJoystickAction;
        public Camera mainCamera;
    }

    
    private readonly UIBattle _view;
    private readonly Deps _deps;

    private UIBattleTopPresenter _topPresenter;
    private UIDashPresenter _dashPresenter;
    private HpBarController _hpBarController;

    public UIBattlePresenter(UIBattle view, Deps deps)
    {
        _view = view;
        _deps = deps;

        _topPresenter = new UIBattleTopPresenter(_view.topView, new UIBattleTopPresenter.Deps
        {
            popupService = deps.popupService,
            sceneService = deps.sceneLoader,
            battleState = deps.battleState,
            battleReward = deps.battleReward
        });

        _dashPresenter = new UIDashPresenter(_view.dashView, deps.dash, deps.onDashAction);

        _hpBarController = view.hpBarController;
        _hpBarController.InitDependencies(deps.mainCamera, deps.assetService);

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
        _view.joystickView.onJoystickMove += _deps.onJoystickAction;

        EventHelper.AddEventListener(EventName.BattleStatus, OnBattleStatus);
    }

    private void Unbind()
    {
        _view.onClickLobby -= OnClickLobby;
        _view.joystickView.onJoystickMove -= _deps.onJoystickAction;

        EventHelper.RemoveEventListener(EventName.BattleStatus, OnBattleStatus);
    }

    public void Initialize()
    {
        _topPresenter.Initialize();
        _dashPresenter.Initialize();
        _hpBarController.Initialize();
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