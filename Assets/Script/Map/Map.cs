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

    public void Init(Transform mapOriginal)
    {
        var map = Instantiate(mapOriginal, transform);
        
        _plane = map.GetChild(0).GetComponent<Tilemap>();
        _plane.CompressBounds();
    }

    public Bounds GetBounds()
    {
        return _plane.localBounds;
    }

    public Vector3 GetRandomPositionAwayFromTarget(Vector3 target)
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
        } while (Vector3.Distance(pos, target) < 3f && attempts < maxAttempts);

        return pos;
    }
}
