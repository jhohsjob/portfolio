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

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    private void LateUpdate()
    {
        _movePosition.x = _player.position.x;
        _movePosition.y = _player.position.y;

        var mapBounds = BattleManager.instance.mapBounds;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        _movePosition.x = Mathf.Clamp(_movePosition.x, mapBounds.min.x + camWidth, mapBounds.max.x - camWidth);
        _movePosition.y = Mathf.Clamp(_movePosition.y, mapBounds.min.y + camHeight, mapBounds.max.y - (camHeight - _topUIHeight));

        transform.position = _movePosition;
    }

    public  void Init(float height)
    {
        _topUIHeight = height;
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
