using UnityEngine;


public class Map : MonoBehaviour
{
    [SerializeField]
    private Transform _plane;

    private float PLANE_SIZE = 5f;
    private Vector3 _mapSize = Vector3.zero;
    
    private void Awake()
    {
        EventHelper.AddEventListener(EventName.GameLevelUp, OnGameLevelUp);
    }

    public void Init()
    {
        SetMapSize();
    }

    private void SetMapSize()
    {
        _mapSize.x = _plane.localScale.x * PLANE_SIZE;
        _mapSize.y = _plane.localScale.y * PLANE_SIZE;
        _mapSize.z = _plane.localScale.z * PLANE_SIZE;

        EventHelper.Send(EventName.ChangeMapSize, this, _mapSize);
    }

    public void SizeChange(Vector3 end)
    {
        _plane.localScale = end;

        SetMapSize();
    }

    private void OnGameLevelUp(object sender, object data)
    {
        if (data == null || (data is GameLevelData) == false)
        {
            return;
        }

        var gameLevelData = data as GameLevelData;

        SizeChange(gameLevelData.growMapSize);
    }
}
