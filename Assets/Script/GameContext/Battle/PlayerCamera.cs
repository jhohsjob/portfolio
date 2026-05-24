using System;
using System.Collections;
using UnityEngine;


public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Transform _player;

    private Vector3 _movePosition = Vector3.back;
    private float _topUIHeight;

    private float _zoomOutTimer = 0f;
    private float _zoomOutSpeed = 1f;

    private Func<Bounds> _getMapBounds;
    private Bounds _mapBounds => _getMapBounds?.Invoke() ?? new Bounds();

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    private void LateUpdate()
    {
        if (_mapBounds.size == Vector3.zero)
        {
            return;
        }

        _movePosition.x = _player.position.x;
        _movePosition.y = _player.position.y;

        float camHeight = _camera.orthographicSize; // Camera.main.orthographicSize;
        float camWidth = camHeight * _camera.aspect; // Camera.main.aspect;

        _movePosition.x = Mathf.Clamp(_movePosition.x, _mapBounds.min.x + camWidth, _mapBounds.max.x - camWidth);
        _movePosition.y = Mathf.Clamp(_movePosition.y, _mapBounds.min.y + camHeight, _mapBounds.max.y - (camHeight - _topUIHeight));

        transform.position = _movePosition;
    }

    public  void Init(float height, Func<Bounds> funcMapBounds)
    {
        _topUIHeight = height;
        _getMapBounds = funcMapBounds;
    }

    public void ZoomOut(int end)
    {
        var start = _camera.orthographicSize;

        StartCoroutine(eZoomOut(start, end));
    }

    private IEnumerator eZoomOut(float start, float end)
    {
        _zoomOutTimer = 0f;

        while (_camera.orthographicSize < end)
        {
            _zoomOutTimer += Time.deltaTime * _zoomOutSpeed;
            _camera.orthographicSize = Mathf.Lerp(start, end, _zoomOutTimer);

            if (_camera.orthographicSize > end)
            {
                _camera.orthographicSize = end;
            }

            yield return null;
        }

        yield return null;
    }
}
