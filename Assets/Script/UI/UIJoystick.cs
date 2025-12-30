using UnityEngine;
using UnityEngine.EventSystems;


public class UIJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform _background;
    [SerializeField]
    private RectTransform _handle;
    [SerializeField]
    private float _radius = 80f;

    private Vector2 _startPos;
    private Vector2 _input;

    private Player _player;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_player == null)
        {
            _player = BattleManager.instance.battleScene.player;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out _startPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, eventData.position, eventData.pressEventCamera, out Vector2 currentPos);

        Vector2 delta = currentPos - _startPos;
        Vector2 clamped = Vector2.ClampMagnitude(delta, _radius);

        _handle.anchoredPosition = clamped;
        _input = clamped / _radius;
        _player?.SetJoystick(_input);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _handle.anchoredPosition = Vector2.zero;
        _input = Vector2.zero;
        _player?.SetJoystick(_input);
    }
}