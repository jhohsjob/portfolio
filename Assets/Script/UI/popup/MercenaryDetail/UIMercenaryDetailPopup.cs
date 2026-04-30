using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIMercenaryDetailPopup : UIPopup, IDragHandler, IEndDragHandler
{
    class UIElement
    {
        public int id;
        public Mercenary data;
        public MercenaryView view;
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

    private int _currentIndex;
    private List<UIElement> _uiElements = new();

    private Dictionary<GameObject, MercenaryViewPool> _pools = new();

    private float _dragThreshold = 150f;
    private Vector2 _startPos;

    protected override void Awake()
    {
        base.Awake();

        _btnLeft.onClick.AddListener(() => OnClickMove(-1));
        _btnRight.onClick.AddListener(() => OnClickMove(1));
    }

    private void Start()
    {
        SetMercenary();
    }

    public override void OnPopupReady(object data = null)
    {
        if (data is not Mercenary mercenary)
        {
            return;
        }

        InitElements();

        _currentIndex = _uiElements.FindIndex(x => x.id == mercenary.id);

        base.OnPopupReady(data);
    }

    public override void Hide()
    {
        foreach (var element in _uiElements)
        {
            var prefab = element.data.original;
            var pool = GetPool(prefab);
            pool.Release(element.view);
        }

        _uiElements.Clear();

        base.Hide();
    }

    private void InitElements()
    {
        _uiElements.Clear();

        var mercenaryList = MercenaryManager.instance.list;

        foreach (var mercenary in mercenaryList)
        {
            var pool = GetPool(mercenary.original);
            var view = pool.Get();

            view.SetActive(false);

            var element = new UIElement
            {
                id = mercenary.id,
                data = mercenary,
                view = view
            };

            _uiElements.Add(element);
        }
    }
    
    private MercenaryViewPool GetPool(GameObject prefab)
    {
        if (_pools.TryGetValue(prefab, out var pool))
        {
            return pool;
        }

        pool = new MercenaryViewPool(prefab, _pivot);
        _pools[prefab] = pool;
        return pool;
    }

    private void SetMercenary()
    {
        foreach (var element in _uiElements)
        {
            element.view.ResetView();
        }

        var current = _uiElements[_currentIndex];
        var data = current.data;
        var view = current.view;

        view.SetActive(true);
        view.SetLocked(data.isOwned == false);

        _txtName.text = data.name;
        _txtAtk.text = $"atk : {data.atk}";
        _txtMaxHp.text = $"hp : {data.maxHP}";
        _txtMoveSpeed.text = $"speed : {data.moveSpeed}";
        _txtDesc.text = data.description;
    }

    private void OnClickMove(int direction)
    {
        _currentIndex = MercenaryManager.instance.CalcIndex(_currentIndex + direction);

        SetMercenary();
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

        if (Mathf.Abs(deltaX) >= _dragThreshold)
        {
            if (deltaX > 0)
            {
                OnClickMove(1);
            }
            else
            {
                OnClickMove(-1);
            }
        }

        _startPos = Vector2.zero;
    }
}
