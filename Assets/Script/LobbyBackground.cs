using UnityEngine;


public class LobbyBackground : MonoBehaviour
{
    [SerializeField]
    private Transform[] _layers;

    private Vector3 _offset;

    private void Start()
    {
        var sr = _layers[0].GetChild(0).GetComponent<SpriteRenderer>();
        _offset = new Vector3(sr.bounds.size.x, 0f, 0f);
    }

    void Update()
    {
        for (int i = 0; i < _layers.Length; i++)
        {
            MoveLayer(_layers[i], 1 + (0.2f * i));
        }
    }

    private void MoveLayer(Transform tf, float speed)
    {
        tf.localPosition += Vector3.right * speed * Time.deltaTime * 3f;

        if (tf.localPosition.x >= _offset.x)
        {
            tf.localPosition -= _offset;
        }
    }
}
