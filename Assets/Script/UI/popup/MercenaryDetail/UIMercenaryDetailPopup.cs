using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIMercenaryDetailPopup : UIPopup, IDragHandler, IEndDragHandler
{
    private class SkillScrollProvider : IScrollDataProvider
    {
        private readonly UIMercenaryDetailPopup _view;

        public SkillScrollProvider(UIMercenaryDetailPopup view)
        {
            _view = view;
        }

        public int GetItemCount()
        {
            return _view.onGetSkillCount?.Invoke() ?? 0;
        }

        public void Bind(int index, InfiniteScrollItem scrollItem)
        {
            if (scrollItem is not UIMercenaryDetailPopupSkillScrollItem item)
            {
                return;
            }

            var mercenary = _view.onGetSkillData?.Invoke(index);
            if (mercenary == null)
            {
                return;
            }

            item.SetData(index, mercenary);
            item.SetOnClick((skill, itemTransform) => _view.onClickSkillItem?.Invoke(skill, itemTransform));
        }
    }

    [SerializeField]
    private Transform _pivot;
    [SerializeField]
    private TextMeshProUGUI _txtName;
    [SerializeField]
    private TextMeshProUGUI _txtAtk;
    [SerializeField]
    private TextMeshProUGUI _txtMaxHp;
    [SerializeField]
    private TextMeshProUGUI _txtMoveSpeed;
    [SerializeField]
    private TextMeshProUGUI _txtDesc;
    [SerializeField]
    private Button _btnLeft;
    [SerializeField]
    private Button _btnRight;
    [SerializeField]
    private HorizontalInfiniteScroll _skillScroll;
    [SerializeField]
    private UIMercenaryDetailPopupSkillInfo _skillInfo;

    private MercenaryView _currentMercenaryView;

    private Dictionary<int, MercenaryViewPool> _pools = new();

    private Vector2 _startPos;

    public event Action onClickLeft;
    public event Action onClickRight;
    public event Action<float> onDrag;

    public Func<int> onGetSkillCount;
    public Func<int, SkillData> onGetSkillData;
    public Action<SkillData, Transform> onClickSkillItem;

    private UIMercenaryDetailPopupPresenter _presenter;

    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIMercenaryDetailPopupPresenter(this);

        _btnLeft.onClick.AddListener(HandleClickLeft);
        _btnRight.onClick.AddListener(HandleClickRight);

        _skillInfo.Hide();
    }

    public override void OnDestroy()
    {
        _presenter?.Dispose();

        foreach (var pool in _pools.Values)
        {
            pool.OnDestroy();
        }
        _pools.Clear();

        base.OnDestroy();
    }
    
    public override void OnPopupReady(object data = null)
    {
        _presenter.OnPopupReady(data, _assetLoader);
        
        base.OnPopupReady(data);
    }

    public void SetupSkillScroll(GameObject prefab, int initIndex)
    {
        _skillScroll.Initialize(
            provider: new SkillScrollProvider(this),
            factory: new SkillItemFactory(prefab),
            itemCount: onGetSkillCount?.Invoke() ?? 0,
            initPos: initIndex
        );
        _skillScroll.UpdateItems();
    }

    public void ShowSkillInfo(SkillData skillData, Transform itemTransform)
    {
        _skillInfo.SetData(skillData);
        _skillInfo.Show(itemTransform);
    }

    private void HandleClickLeft()
    {
        onClickLeft?.Invoke();
    }

    private void HandleClickRight()
    {
        onClickRight?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_startPos == Vector2.zero)
        {
            _startPos = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - _startPos.x;

        onDrag?.Invoke(deltaX);

        _startPos = Vector2.zero;
    }

    public void ShowMercenary(Mercenary mercenary)
    {
        _currentMercenaryView?.SetActive(false);
        
        var view = GetPool(mercenary.id, mercenary.original).Get();

        view.SetActive(true);
        view.SetLocked(!mercenary.isOwned);

        _currentMercenaryView = view;

        _txtName.text = mercenary.name;
        _txtAtk.text = $"atk : {mercenary.atk}";
        _txtMaxHp.text = $"hp : {mercenary.maxHP}";
        _txtMoveSpeed.text = $"speed : {mercenary.moveSpeed}";
        _txtDesc.text = mercenary.description;

        _skillScroll.UpdateItems();
    }

    private MercenaryViewPool GetPool(int id, GameObject original)
    {
        if (_pools.TryGetValue(id, out var pool))
        {
            return pool;
        }

        pool = new MercenaryViewPool(original, _pivot);

        _pools.Add(id, pool);

        return pool;
    }
}
