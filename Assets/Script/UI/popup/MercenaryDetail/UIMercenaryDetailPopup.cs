using System;
using System.Collections.Generic;
using System.Linq;
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
    private UIStat _offensiveStat;
    [SerializeField]
    private UIStat _defensiveStat;
    [SerializeField]
    private UIStat _utilityStat;
    [SerializeField]
    private TextMeshProUGUI _txtCombatPower;

    [SerializeField]
    private UIButton _btnLevelUp;
    [SerializeField]
    private UILongPressButton _longBtnLevelUp;
    [SerializeField]
    private UIButton _btnLevelUpMax;
    [SerializeField]
    private UIButton _btnLevelReset;
    [SerializeField]
    private Button _btnLeft;
    [SerializeField]
    private Button _btnRight;
    [SerializeField]
    private HorizontalInfiniteScroll _skillScroll;
    [SerializeField]
    private UIMercenaryDetailPopupSkillInfo _skillInfo;

    [SerializeField]
    private GameObject _gradeRoot;
    private Image[] _gradeIcons;

    private MercenaryService _mercenaryService;
    private User _user; 

    private MercenaryView _currentMercenaryView;

    private Dictionary<int, MercenaryViewPool> _pools = new();

    private Vector2 _startPos;

    public event Action onClickLevelUp;
    public event Action<int> onLongPressLevelUp;
    public event Action onClickLevelUpMax;
    public event Action onClickLevelReset;
    public event Action onClickLeft;
    public event Action onClickRight;
    public event Action<float> onDrag;

    public Func<int> onGetSkillCount;
    public Func<int, SkillData> onGetSkillData;
    public Action<SkillData, Transform> onClickSkillItem;

    private UIMercenaryDetailPopupPresenter _presenter;

    private int _previewLevel;

    protected override void Awake()
    {
        base.Awake();

        _presenter = new UIMercenaryDetailPopupPresenter(this);

        _skillInfo.Hide();

        Bind();

        _gradeIcons = _gradeRoot.GetComponentsInChildren<Image>();
        if (_gradeIcons.Length != GameConfig.MercenaryGradeLimit)
        {
            Debug.LogWarning("grade miss match");
        }
    }

    public override void OnDestroy()
    {
        _presenter?.Dispose();

        foreach (var pool in _pools.Values)
        {
            pool.OnDestroy();
        }
        _pools.Clear();

        Unbind();

        base.OnDestroy();
    }

    private void Bind()
    {
        _btnLevelUpMax.AddListener(HandleClickLevelUpMax);
        _btnLevelReset.AddListener(HandleClickLevelReset);
        _btnLeft.onClick.AddListener(HandleClickLeft);
        _btnRight.onClick.AddListener(HandleClickRight);

        _longBtnLevelUp.onClick += HandleClickLevelUp;
        _longBtnLevelUp.onLongPressStart += HandleLongPressStart;
        _longBtnLevelUp.onLongPressLevelUp += HandleLongPressLevelUp;
        _longBtnLevelUp.onLongPressEnd += HandleLongPressEnd;
    }

    private void Unbind()
    {
        _btnLevelUpMax.RemoveListener(HandleClickLevelUpMax);
        _btnLevelReset.RemoveListener(HandleClickLevelReset);
        _btnLeft.onClick.RemoveListener(HandleClickLeft);
        _btnRight.onClick.RemoveListener(HandleClickRight);

        _longBtnLevelUp.onClick += HandleClickLevelUp;
        _longBtnLevelUp.onLongPressStart -= HandleLongPressStart;
        _longBtnLevelUp.onLongPressLevelUp -= HandleLongPressLevelUp;
        _longBtnLevelUp.onLongPressEnd -= HandleLongPressEnd;
    }

    public void AddDependencies(MercenaryService mercenaryService, User user)
    {
        _mercenaryService = mercenaryService;
        _user = user;
    }

    public override void OnPopupReady(object data = null)
    {
        _presenter.InitDependencies(_assetLoader, _popupService, _mercenaryService, _user);
        _presenter.OnPopupReady(data);

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

    private void HandleClickLevelUp()
    {
        onClickLevelUp?.Invoke();
    }

    private void HandleLongPressStart()
    {
        _previewLevel = _presenter.currentLevel;
    }

    private void HandleLongPressLevelUp()
    {
        if (_presenter.CanPreviewLevelUp(_previewLevel) == false)
        {
            return;
        }
        _previewLevel++;

        _btnLevelUp.text = $"Lv. {_previewLevel}";
    }

    private void HandleLongPressEnd()
    {
        onLongPressLevelUp?.Invoke(_previewLevel);
    }

    private void HandleClickLevelUpMax()
    {
        onClickLevelUpMax?.Invoke();
    }

    private void HandleClickLevelReset()
    {
        onClickLevelReset?.Invoke();
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

    public void ShowMercenary(Mercenary mercenary, int availableMaxLevel)
    {
        _currentMercenaryView?.SetActive(false);
        
        var view = GetPool(mercenary.id, mercenary.original).Get();

        view.SetActive(true);
        view.SetLocked(!mercenary.isOwned);

        _currentMercenaryView = view;

        _txtName.text = mercenary.name;

        _offensiveStat.Refresh(mercenary.stat.offensive);
        _defensiveStat.Refresh(mercenary.stat.defensive);
        _utilityStat.Refresh(mercenary.stat.utility);

        _txtCombatPower.text = CombatPowerCalculator.Calculate(mercenary.stat).ToString();

        var canLevelUp = _presenter.CanLevelUp();
        _btnLevelUp.interactable = canLevelUp;
        _longBtnLevelUp.interactable = canLevelUp;
        _btnLevelUpMax.interactable = canLevelUp;
        _btnLevelReset.interactable = _presenter.CanLevelReset();

        _previewLevel = mercenary.level;

        _btnLevelUp.text = $"Lv. {mercenary.level}";
        _btnLevelUpMax.text = $"Max Lv. {availableMaxLevel}";

        _skillScroll.UpdateItems();

        for (int i = 0; i < _gradeIcons.Length; i++)
        {
            _gradeIcons[i].gameObject.SetActive(i < mercenary.grade);
        }
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
