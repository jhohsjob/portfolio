using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIGachaResultPopupDependencies : UIPopupDependencies
{
    public MercenaryService mercenaryService;
    public GachaService gachaService;
}

public class UIGachaResultPopupData
{
    public int gachaId;
    public IReadOnlyList<RewardBase> rewards;

    public UIGachaResultPopupData(int gachaId, IReadOnlyList<RewardBase> rewards)
    {
        this.gachaId = gachaId;
        this.rewards = rewards;
    }
}

public class UIGachaResultPopup : UIPopup
{
    private class GachaResultScrollProvider : IScrollDataProvider
    {
        private readonly UIGachaResultPopup _view;
        private readonly MercenaryService _mercenaryService;

        public GachaResultScrollProvider(UIGachaResultPopup view, MercenaryService mercenaryService)
        {
            _view = view;
            _mercenaryService = mercenaryService;
        }

        public int GetItemCount()
        {
            return _view.onGetItemCount?.Invoke() ?? 0;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            var reward = _view.onGetReward?.Invoke(index);
            if (reward == null || reward is not MercenaryReward mercenaryReward)
            {
                return;
            }

            MercenaryDefinition mercenary = _mercenaryService.GetMercDefinitionById(mercenaryReward.mercenaryId);

            item.SetData(index, mercenary);
        }
    }

    [SerializeField]
    private VerticalInfiniteScroll _scroll;

    [SerializeField]
    private Button _btnGacha01;
    [SerializeField]
    private Button _btnGacha10;

    public Func<int, RewardBase> onGetReward;
    public Func<int> onGetItemCount;

    private MercenaryService _mercenaryService;

    public event Action<int> onClickGacha; 

    private UIGachaResultPopupPresenter _presenter;

    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIGachaResultPopupPresenter(this);

        Bind();
    }

    public override void OnDestroy()
    {
        _presenter?.Dispose();

        Unbind();

        base.OnDestroy();
    }

    private void Bind()
    {
        _btnGacha01.onClick.AddListener(HandleClickGacha01);
        _btnGacha10.onClick.AddListener(HandleClickGacha10);
    }

    private void Unbind()
    {
        _btnGacha01.onClick.RemoveListener(HandleClickGacha01);
        _btnGacha10.onClick.RemoveListener(HandleClickGacha10);
    }

    public override void InitDependencies(UIPopupDependencies dependencies)
    {
        base.InitDependencies(dependencies);

        if (dependencies is UIGachaResultPopupDependencies gachaDependencies)
        {
            _mercenaryService = gachaDependencies.mercenaryService;

            _presenter.InitDependencies(gachaDependencies);
        }
    }

    public override void OnPopupReady(object data = null)
    {
        base.OnPopupReady(data);

        if (data is not UIGachaResultPopupData popupData)
        {
            return;
        }

        _presenter.OnPopupReady(popupData);
    }

    public void SetupScroll(GameObject prefab)
    {
        var factory = new ScrollItemFactory(prefab);
        _scroll.Initialize(
            provider: new GachaResultScrollProvider(this, _mercenaryService),
            factory: new ScrollItemFactory(prefab)
        );
    }

    public void OnGachaCallback()
    {
        _scroll.UpdateItems();

        _btnGacha01.interactable = true;
        _btnGacha10.interactable = true;
        _btnClose.interactable = true;
    }

    private void HandleClickGacha01()
    {
        _btnGacha01.interactable = false;
        _btnGacha10.interactable = false;
        _btnClose.interactable = false;

        onClickGacha?.Invoke(1);
    }

    private void HandleClickGacha10()
    {
        _btnGacha01.interactable = false;
        _btnGacha10.interactable = false;
        _btnClose.interactable = false;

        onClickGacha?.Invoke(10);
    }
}