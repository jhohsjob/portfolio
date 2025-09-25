using UnityEngine;
using UnityEngine.Tilemaps;


public class Map : MonoBehaviour
{
    private Tilemap _plane;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }

    public void Init()
    {
        var map = Instantiate(BattleManager.instance.mapLevelManager.mapOriginal, transform);
        
        _plane = map.GetChild(0).GetComponent<Tilemap>();
        _plane.CompressBounds();

        SetMapSize();
    }

    private void SetMapSize()
    {
        EventHelper.Send(EventName.ChangeMapSize, this, _plane.localBounds);
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
}
