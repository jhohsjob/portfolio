using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class UILongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    private float _longPressDelay = 0.5f;

    [SerializeField]
    private float _repeatInterval = 0.1f;

    private Coroutine _coroutine;

    private bool _isPointerDown;
    private bool _isLongPress;

    public bool interactable;

    public event Action onClick;
    public event Action onLongPressStart;
    public event Action onLongPressLevelUp;
    public event Action onLongPressEnd;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (interactable == false)
        {
            return;
        }

        if (_isPointerDown)
        {
            return;
        }

        _isPointerDown = true;
        _isLongPress = false;

        _coroutine = StartCoroutine(LongPressCoroutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (interactable == false)
        {
            return;
        }

        End();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable == false)
        {
            return;
        }

        End();
    }

    private IEnumerator LongPressCoroutine()
    {
        yield return new WaitForSeconds(_longPressDelay);

        if (_isPointerDown == false)
        {
            yield break;
        }

        _isLongPress = true;

        onLongPressStart?.Invoke();

        while (_isPointerDown)
        {
            onLongPressLevelUp?.Invoke();

            yield return new WaitForSeconds(_repeatInterval);
        }
    }

    private void End()
    {
        if (_isPointerDown == false)
        {
            return;
        }

        _isPointerDown = false;

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        if (_isLongPress)
        {
            onLongPressEnd?.Invoke();
        }
        else
        {
            onClick?.Invoke();
        }

        _isLongPress = false;
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}