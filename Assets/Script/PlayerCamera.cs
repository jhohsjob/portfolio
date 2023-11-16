using System.Collections;
using UnityEngine;


public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private Transform _player;

    private Vector3 _movePosition = new Vector3(0f, 10f, 0f);

    private float _zoomOutTimer = 0f;
    private float _zoomOutSpeed = 1f;

    private void Awake()
    {
        EventHelper.AddEventListener(EventName.MapLevelUp, OnGameLevelUp);
    }

    private void LateUpdate()
    {
        _movePosition.x = _player.position.x;
        _movePosition.z = _player.position.z;

        transform.position = _movePosition;
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

    private void OnGameLevelUp(object sender, object data)
    {
        if (data == null || (data is MapLevel) == false)
        {
            return;
        }

        var gameLevelData = data as MapLevel;

        ZoomOut(gameLevelData.growCameraSize);
    }
}
