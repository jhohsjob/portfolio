using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UILobbyMiddleGacha : UILobbyMiddleBase
{
    private class GachaScrollProvider : IScrollDataProvider
    {
        private readonly UILobbyMiddleGacha _view;

        public GachaScrollProvider(UILobbyMiddleGacha view)
        {
            _view = view;
        }

        public int GetItemCount()
        {
            return _view.onGetItemCount?.Invoke() ?? 0;
        }

        public void Bind(int index, InfiniteScrollItem item)
        {
            var gacha = _view.onGetGachaData?.Invoke(index);
            if (gacha == null)
            {
                return;
            }

            item.SetData(index, gacha);

            if (item is UIGachaScrollItem gachaItem)
            {
                var scrollUI = _view.GetScrollItem(gacha.id, gacha.scrollUI);
                gachaItem.SetUI(scrollUI);
            }
        }
    }

    [SerializeField]
    private HorizontalInfiniteScroll _gachaScroll;
    [SerializeField]
    private Transform _mainLayer;

    [SerializeField]
    private Button _btnGacah01;
    [SerializeField]
    private Button _btnGacah10;

    public Func<int> onGetItemCount;
    public Func<int, Gacha> onGetGachaData;
    public Action<int> onGachaSelected;

    public Action<int> onGacha;

    public int listIndex => _gachaScroll.GetCenteredIndex();

    private readonly Dictionary<int, GameObject> _mainUIDic = new();
    private readonly Dictionary<int, GameObject> _scrollUIDic = new();

    private GameObject _mainUI;

    protected override void Awake()
    {
        base.Awake();

        _gachaScroll.OnScrollStopped += OnScrollStopped;

        _btnGacah01.onClick.AddListener(OnClickGacha01);
        _btnGacah10.onClick.AddListener(OnClickGacha10);
    }

    public void SetupGachaScroll(GameObject prefab)
    {
        _gachaScroll.Initialize(
            provider: new GachaScrollProvider(this),
            factory: new ScrollItemFactory(prefab)
        );
        _gachaScroll.UpdateItems();
    }

    public void UpdateMainLayer(int gachaId, GameObject original)
    {
        if (original == null)
        {
            return;
        }

        if (_mainUIDic.TryGetValue(gachaId, out GameObject mainUI) == false)
        {
            mainUI = Instantiate(original, _mainLayer);
            _mainUIDic.Add(gachaId, mainUI);
        }

        if (_mainUI != null)
        {
            _mainUI.SetActive(false);
        }
        
        _mainUI = mainUI;
        _mainUI.SetActive(true);
    }

    public GameObject GetScrollItem(int gachaId, GameObject original)
    {
        if (original == null)
        {
            return null;
        }

        if (_scrollUIDic.TryGetValue(gachaId, out GameObject scroll) == false)
        {
            scroll = Instantiate(original);
            _scrollUIDic.Add(gachaId, scroll);
            scroll.SetActive(false);
        }

        return scroll;
    }

    private void OnScrollStopped(int index)
    {
        onGachaSelected.Invoke(index);
    }

    private void OnClickGacha01()
    {
        onGacha?.Invoke(1);
    }

    private void OnClickGacha10()
    {
        onGacha?.Invoke(10);
    }
}
