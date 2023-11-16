using UnityEngine;


public class Map : MonoBehaviour
{
    [SerializeField]
    private Transform _plane;

    private float PLANE_SIZE = 5f;
    private Vector3 _mapSize = Vector3.zero;
    
    private void Awake()
    {
        EventHelper.AddEventListener(EventName.MapLevelUp, OnGameLevelUp);
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

    public Vector3 GetRandomPos()
    {
        var x = Random.Range(-_mapSize.x, _mapSize.x);
        var z = Random.Range(-_mapSize.z, _mapSize.z);

        return new Vector3(x, 0, z);
    }

    private void OnGameLevelUp(object sender, object data)
    {
        if (data == null || (data is MapLevel) == false)
        {
            return;
        }

        var mapLevel = data as MapLevel;

        SizeChange(mapLevel.growMapSize);
    }
}
