using System;
using UnityEngine;


public class UILobbyPresenter : IDisposable
{
    public class Deps
    {
        public IAssetService assetService;
        public ILocaleService localeService;
        public IDeviceSettings deviceSettings;
        public User user;
        public ICurrencyService currencyService;
        public MercenaryService mercenaryService;
        public PurchaseService purchaseService;
        public GachaService gachaService;
        public StageService stageService;
        public ISceneService sceneService;
        public IPopupService popupService;
    }


    private readonly UILobby _view;
    private readonly Deps _deps;

    private UILobbyTopPresenter _topPresenter;
    private UILobbyMiddleBasePresenter[] _middlePresenters;

    public UILobbyPresenter(UILobby view, Deps deps)
    {
        _view = view;
        _deps = deps;

        _topPresenter = new UILobbyTopPresenter(_view.topView, new UILobbyTopPresenter.Deps
        {
            localeService = _deps.localeService,
            deviceSettings = _deps.deviceSettings,
            user = _deps.user,
            currencyService = _deps.currencyService,
            sceneService = _deps.sceneService,
            popupService = _deps.popupService,
        });

        _middlePresenters = new UILobbyMiddleBasePresenter[_view.middleViews.Length];
        for (int i = 0; i < _view.middleViews.Length; i++)
        {
            if (_view.middleViews[i] is UILobbyMiddleBarrack barrack)
            {
                _middlePresenters[i] = new UILobbyMiddleBarrackPresenter(barrack, deps);
            }
            else if (_view.middleViews[i] is UILobbyMiddleBattle battle)
            {
                _middlePresenters[i] = new UILobbyMiddleBattlePresenter(battle, deps);
            }
            else if (_view.middleViews[i] is UILobbyMiddleShop shop)
            {
                _middlePresenters[i] = new UILobbyMiddleShopPresenter(shop, deps);
            }
            else if (_view.middleViews[i] is UILobbyMiddleGacha gacha)
            {
                _middlePresenters[i] = new UILobbyMiddleGachaPresenter(gacha, deps);
            }
        }

        Bind();
    }

    public void Dispose()
    {
        _topPresenter.Dispose();
        
        foreach (var presenter in _middlePresenters)
        {
            presenter?.Dispose();
        }

        Unbind();
    }

    private void Bind()
    {
        _view.onClickBottomMenu += OnClickBottomMenu;
    }

    private void Unbind()
    {
        _view.onClickBottomMenu -= OnClickBottomMenu;
    }

    public void Initialize()
    {
        _topPresenter.Initialize();
        foreach (var presenter in  _middlePresenters)
        {
            presenter?.Initialize();
        }

        _view.HideAllMiddle();
        _view.ShowMiddle(((int)LobbyMenu.Battle));
    }

    private void OnClickBottomMenu(int index)
    {
        if (index >= _view.menuCount || index >= _view.GetMiddleCount())
        {
            Debug.Log("menu mismatch");
            return;
        }

        _view.HideAllMiddle();
        _view.ShowMiddle(index);
    }
}