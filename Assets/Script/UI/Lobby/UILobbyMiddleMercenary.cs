using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UILobbyMiddleMercenary : UILobbyMiddle
{
    struct UIElement
    {
        public int id;
        public Mercenary data;
        public GameObject body;
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
    private Button _btnSelect;
    [SerializeField]
    private Button _btnLeft;
    [SerializeField]
    private Button _btnRight;

    private int _currentIndex;
    private List<UIElement> uiElements = new List<UIElement>();

    private float _dragThreshold = 150f;
    private Vector2 _startPos;

    protected override void Awake()
    {
        base.Awake();

        _btnSelect.onClick.AddListener(OnClickSelect);
        _btnLeft.onClick.AddListener(() => OnClickMove(-1));
        _btnRight.onClick.AddListener(() => OnClickMove(1));

        uiElements.Clear();

        var mercenaryList = MercenaryHander.instance.list;
        foreach (var mercenary in mercenaryList)
        {
            var uiData = new UIElement
            {
                id = mercenary.id,
                data = mercenary,
                body = Instantiate(mercenary.original, _pivot)
            };
            uiElements.Add(uiData);
        }
    }

    protected override void OnEnter()
    {
        _currentIndex = uiElements.FindIndex(x => x.id == Client.user.mercenaryId);

        SetMercenary();
    }

    private void SetMercenary()
    {
        foreach (var body in uiElements)
        {
            body.body.SetActive(false);
        }
        uiElements[_currentIndex].body.SetActive(true);

        var mercenary = uiElements[_currentIndex].data;

        _txtName.text = mercenary.name;
        _txtAtk.text = $"atk : {mercenary.atk}";
        _txtMaxHp.text = $"hp : {mercenary.maxHP}";
        _txtMoveSpeed.text = $"atk : {mercenary.moveSpeed}";
        _txtDesc.text = mercenary.description;
    }

    private void OnClickSelect()
    {
        Client.user.SetMercenary(uiElements[_currentIndex].id);
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
