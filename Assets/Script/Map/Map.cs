using UnityEngine;
using UnityEngine.Tilemaps;


public class Map : MonoBehaviour
{
    [SerializeField]
    private Tilemap _plane;

    private void Awake()
    {
        _plane.CompressBounds();

        EventHelper.AddEventListener(EventName.MapLevelUp, OnMapLevelUp);
    }

    private void OnDestroy()
    {
        EventHelper.RemoveEventListener(EventName.MapLevelUp, OnMapLevelUp);
    }

    public void Init()
    {
        SetMapSize();
    }

    private void SetMapSize()
    {
        EventHelper.Send(EventName.ChangeMapSize, this, _plane.localBounds);
    }

    public void SizeChange(Vector3 end)
    {
        //_plane.localScale = end;

        SetMapSize();
    }

    public Vector3 GetRandomPos(Vector3 player)
    {
        Vector3 pos;
        int maxAttempts = 100;
        int attempts = 0;

        do
        {
            float x = Random.Range(_plane.localBounds.min.x, _plane.localBounds.max.x);
            float y = Random.Range(_plane.localBounds.min.y, _plane.localBounds.max.y);
            pos = new Vector3(x, y, 0f);

            attempts++;
        } while (Vector3.Distance(pos, player) < 3f && attempts < maxAttempts);

        return pos;
    }

    private void OnMapLevelUp(object sender, object data)
    {
        if (data == null || (data is MapLevel) == false)
        {
            return;
        }

        var mapLevel = data as MapLevel;

        // SizeChange(mapLevel.growMapSize);
    }
}
