using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectMercenary : UIPopup, IDragHandler, IEndDragHandler
{
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
    private Button _btnSelect;
    [SerializeField]
    private Button _btnLeft;
    [SerializeField]
    private Button _btnRight;

    private Mercenary _mercenary;

    private int _currentIndex;
    private Dictionary<int, GameObject> _body = new Dictionary<int, GameObject>();

    private float _dragThreshold = 150f;
    private Vector2 _startPos;

    protected override void Awake()
    {
        base.Awake();

        _btnSelect.onClick.AddListener(OnClickSelect);
        _btnLeft.onClick.AddListener(() => OnClickMove(-1));
        _btnRight.onClick.AddListener(() => OnClickMove(1));
    }

    protected override void Init(object data = null)
    {
        _currentIndex = 0;
        
        SetMercenary();
    }

    private void SetMercenary()
    {
        _mercenary = MercenaryHander.instance.GetMercenaryByIndex(_currentIndex);

        foreach (var body in _body.Values)
        {
            body.SetActive(false);
        }
        if (_body.ContainsKey(_currentIndex))
        {
            _body[_currentIndex].SetActive(true);
        }
        else
        {
            var body = Instantiate(Resources.Load<GameObject>(_mercenary.resourcePath), _pivot);
            _body.Add(_currentIndex, body);
        }

        _txtName.text = _mercenary.name;
        _txtAtk.text = $"atk : {_mercenary.atk}";
        _txtMaxHp.text = $"hp : {_mercenary.maxHP}";
        _txtMoveSpeed.text = $"atk : {_mercenary.moveSpeed}";
        _txtDesc.text = _mercenary.description;
    }

    private void OnClickSelect()
    {
        User.instance.SetMercenary(_mercenary.id);

        Hide();
    }

    private void OnClickMove(int direction)
    {
        _currentIndex = MercenaryHander.instance.CaclIndex(_currentIndex + direction);

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
