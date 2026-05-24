using System;
using UnityEngine;


public class UILobbyContext
{
    public IAssetLoader assetLoader;
    public IPopupService popupService;
    public ICurrencyService currencyService;
    public ISceneLoader sceneLoader;
    public StageService stageService;
    public User user;
    public ProductStorage productStorage;
    public PurchaseService purchaseService;
}

public class UILobbyPresenter : IDisposable
{
    private readonly UILobby _view;
    private readonly UILobbyContext _context;

    private UILobbyTopPresenter _topPresenter;
    private UILobbyMiddleBasePresenter[] _middlePresenters;

    public UILobbyPresenter(UILobby view, UILobbyContext context)
    {
        _view = view;
        _context = context;

        _topPresenter = new UILobbyTopPresenter(_view.TopView, new UILobbyTopContext
        {
            popupService = _context.popupService,
            currencyService = _context.currencyService,
        });

        _middlePresenters = new UILobbyMiddleBasePresenter[_view.MiddleViews.Length];
        for (int i = 0; i < _view.MiddleViews.Length; i++)
        {
            if (_view.MiddleViews[i] is UILobbyMiddleBarrack barrack)
            {
                _middlePresenters[i] = new UILobbyMiddleBarrackPresenter(barrack, context);
            }
            else if (_view.MiddleViews[i] is UILobbyMiddleBattle battle)
            {
                _middlePresenters[i] = new UILobbyMiddleBattlePresenter(battle, context);
            }
            else if (_view.MiddleViews[i] is UILobbyMiddleShop shop)
            {
                _middlePresenters[i] = new UILobbyMiddleShopPresenter(shop, context);
            }
        }

        Bind();
    }

    private void Bind()
    {
        _view.onClickBottomMenu += OnClickBottomMenu;
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

    public void Dispose()
    {
        _view.onClickBottomMenu -= OnClickBottomMenu;

        _topPresenter.Dispose();
        foreach (var presenter in _middlePresenters)
        {
            presenter?.Dispose();
        }
    }

    private void OnClickBottomMenu(int index)
    {
        if (index >= _view.menuCount || index >= _view.GetMiddleCount())
        {
            Debug.Log("menu miss match");
            return;
        }

        _view.HideAllMiddle();
        _view.ShowMiddle(index);
    }
}