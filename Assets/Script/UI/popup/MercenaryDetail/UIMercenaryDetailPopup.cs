using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIMercenaryDetailPopupDependencies : UIPopupDependencies
{
    public User user;
    public ICurrencyService currencyService;
    public MercenaryService mercenaryService;
}

public struct MercenaryViewState
{
    public int availableMaxLevel { get; set; }
    public bool canLevelUp { get; set; }
    public bool canLevelReset { get; set; }
    public bool canGradeUp { get; set; }
    public int gradeUpCost { get; set; }
}

public class UIMercenaryDetailPopup : UIPopup, IBeginDragHandler, IDragHandler, IEndDragHandler
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

            var skillData = _view.onGetSkillData?.Invoke(index);
            if (skillData == null)
            {
                return;
            }

            item.SetData(index, skillData);
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
    private UIButton _btnGradeUp;
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

    private float _dragStartX;
    private bool _skillScrollReady;

    public event Action onClickLevelUp;
    public event Action<int> onLongPressLevelUp;
    public event Action onClickLevelUpMax;
    public event Action onClickLevelReset;
    public event Action onClickGradeUp;
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

        _presenter = new UIMercenaryDetailPopupPresenter(this, _pivot);

        _skillInfo.Hide();

        Bind();

        _gradeIcons = _gradeRoot.GetComponentsInChildren<Image>(true);
        if (_gradeIcons.Length != GameConfig.MercenaryGradeLimit)
        {
            Debug.LogWarning("grade miss match");
        }
    }

    public override void OnDestroy()
    {
        _presenter?.Dispose();

        Unbind();

        base.OnDestroy();
    }

    private void Bind()
    {
        _btnLevelUpMax.AddListener(HandleClickLevelUpMax);
        _btnLevelReset.AddListener(HandleClickLevelReset);
        _btnGradeUp.AddListener(HandleClickGradeUp);
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
        _btnGradeUp.RemoveListener(HandleClickGradeUp);
        _btnLeft.onClick.RemoveListener(HandleClickLeft);
        _btnRight.onClick.RemoveListener(HandleClickRight);

        _longBtnLevelUp.onClick -= HandleClickLevelUp;
        _longBtnLevelUp.onLongPressStart -= HandleLongPressStart;
        _longBtnLevelUp.onLongPressLevelUp -= HandleLongPressLevelUp;
        _longBtnLevelUp.onLongPressEnd -= HandleLongPressEnd;
    }

    public override void InitDependencies(UIPopupDependencies dependencies)
    {
        base.InitDependencies(dependencies);

        if (dependencies is UIMercenaryDetailPopupDependencies mercenaryDependencies)
        {
            _presenter.InitDependencies(mercenaryDependencies);
        }
    }

    public override void OnPopupReady(object data = null)
    {
        base.OnPopupReady(data);

        _presenter.OnPopupReady(data);
    }

    public override void Show(object data = null)
    {
        base.Show(data);

        _presenter.Show();
    }

    public void SetupSkillScroll(GameObject prefab, int initIndex)
    {
        _skillScroll.Initialize(
            provider: new SkillScrollProvider(this),
            factory: new ScrollItemFactory(prefab),
            initPos: initIndex
        );
        _skillScroll.UpdateItems();

        _skillScrollReady = true;
    }

    public void ShowSkillInfo(SkillData skillData, Transform itemTransform)
    {
        _skillInfo.SetData(skillData);
        _skillInfo.Show(itemTransform);
    }

    #region Handlers

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
        if (_previewLevel > _presenter.currentLevel)
        {
            onLongPressLevelUp?.Invoke(_previewLevel);
        }
    }

    private void HandleClickLevelUpMax()
    {
        onClickLevelUpMax?.Invoke();
    }

    private void HandleClickLevelReset()
    {
        onClickLevelReset?.Invoke();
    }

    private void HandleClickGradeUp()
    {
        onClickGradeUp?.Invoke();
    }

    private void HandleClickLeft()
    {
        onClickLeft?.Invoke();
    }

    private void HandleClickRight()
    {
        onClickRight?.Invoke();
    }

    #endregion

    #region Drag

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartX = eventData.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.position.x - _dragStartX);
    }

    #endregion

    public void ShowMercenary(Mercenary mercenary, MercenaryView view, in MercenaryViewState state)
    {
        view.SetLocked(!mercenary.isOwned);

        _skillInfo.Hide();

        _txtName.text = mercenary.name;

        _offensiveStat.Refresh(mercenary.stat.offensive);
        _defensiveStat.Refresh(mercenary.stat.defensive);
        _utilityStat.Refresh(mercenary.stat.utility);

        _txtCombatPower.text = CombatPowerCalculator.Calculate(mercenary.stat).ToString();

        _btnLevelUp.interactable = state.canLevelUp;
        _longBtnLevelUp.interactable = state.canLevelUp;
        _btnLevelUpMax.interactable = state.canLevelUp;
        _btnLevelReset.interactable = state.canLevelReset;
        _btnGradeUp.interactable = state.canGradeUp;

        _previewLevel = mercenary.level;

        _btnLevelUp.text = $"Lv. {mercenary.level}";
        _btnLevelUpMax.text = $"Max Lv. {state.availableMaxLevel}";

        if (mercenary.grade == GameConfig.MercenaryGradeLimit)
        {
            _btnGradeUp.text = "Grade Max";
        }
        else
        {
            _btnGradeUp.text = $"Grade Up\n({mercenary.shardCount} / {state.gradeUpCost})";
        }

        if (_skillScrollReady == true)
        {
            _skillScroll.UpdateItems();
        }

        for (int i = 0; i < _gradeIcons.Length; i++)
        {
            _gradeIcons[i].gameObject.SetActive(i < mercenary.grade);
        }
    }
}