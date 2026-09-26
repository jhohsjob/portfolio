using UnityEngine;
using UnityEngine.EventSystems;


public class UIDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _target;
    private Canvas _canvas;

    private bool _isDragging;
    private bool _wasDragged;

    public bool isDragging => _isDragging;
    public bool wasDragged => _wasDragged;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void SetTarget(RectTransform target)
    {
        _target = target;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        _wasDragged = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            _canvas.worldCamera,
            out var localPoint))
        {
            _target.anchoredPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
    }

    public bool ConsumeWasDragged()
    {
        if (_wasDragged == false)
        {
            return false;
        }

        _wasDragged = false;
        return true;
    }
}