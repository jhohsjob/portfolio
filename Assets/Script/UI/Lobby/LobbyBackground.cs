using System;
using UnityEngine;


public class LobbyBackground : MonoBehaviour
{
    [Serializable]
    private class Layer
    {
        [Tooltip("좌우로 이어 붙여도 이음새가 없는 스프라이트")]
        public Sprite sprite;
        [Tooltip("스프라이트용 머티리얼 (예: Sprite-Unlit-Default / Sprite-Lit-Default)")]
        public Material material;
        public string sortingLayerName = "Default";
        public int sortingOrder;
        public Color color = Color.white;
        [Tooltip("기본 속도에 곱해지는 배율")]
        public float speedMultiplier = 1f;
        [Tooltip("이 오브젝트 기준 로컬 위치")]
        public Vector3 localPosition;

        [NonSerialized] public RenderParams renderParams;
        [NonSerialized] public SpriteParams spriteParams;
        [NonSerialized] public Bounds bounds;
        [NonSerialized] public float width;
        [NonSerialized] public float scroll;
        [NonSerialized] public bool valid;
    }

    private const int MaxTilesPerLayer = 16;

    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _baseSpeed = 3f;
    [SerializeField]
    private bool _useUnscaledTime = false;
    [SerializeField]
    private Layer[] _layers;

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        foreach (var layer in _layers)
        {
            Setup(layer);
        }
    }

    private void Setup(Layer layer)
    {
        layer.valid = false;

        if (layer.sprite == null || layer.material == null)
        {
            Debug.LogWarning($"[LobbyBackground] sprite 또는 material이 비어 있습니다.");
            return;
        }

        layer.bounds = layer.sprite.bounds;
        layer.width = layer.bounds.size.x;
        if (layer.width <= 0f)
        {
            return;
        }

        layer.renderParams = new RenderParams(layer.material)
        {
            camera = _camera,
            layer = gameObject.layer,
            sortingLayerID = SortingLayer.NameToID(layer.sortingLayerName),
            sortingOrder = layer.sortingOrder,
        };
        layer.spriteParams = new SpriteParams(layer.sprite, layer.color, SpriteMaskInteraction.None);

        layer.scroll = 0f;
        layer.valid = true;
    }

    private void Update()
    {
        if (_camera == null)
        {
            return;
        }

        float dt = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        // 카메라 시야의 좌우 끝을 이 오브젝트의 로컬 x로 변환
        float halfHeight = _camera.orthographicSize;
        float halfWidth = halfHeight * _camera.aspect;
        Vector3 camPos = _camera.transform.position;

        float a = transform.InverseTransformPoint(camPos - new Vector3(halfWidth, 0f, 0f)).x;
        float b = transform.InverseTransformPoint(camPos + new Vector3(halfWidth, 0f, 0f)).x;
        float left = Mathf.Min(a, b);
        float right = Mathf.Max(a, b);

        Matrix4x4 parentToWorld = transform.localToWorldMatrix;

        foreach (var layer in _layers)
        {
            if (layer.valid == false)
            {
                continue;
            }

            DrawLayer(layer, dt, left, right, parentToWorld);
        }
    }

    private void DrawLayer(Layer layer, float dt, float left, float right, in Matrix4x4 parentToWorld)
    {
        float w = layer.width;

        layer.scroll = Mathf.Repeat(layer.scroll + _baseSpeed * layer.speedMultiplier * dt, w);

        // 타일 k의 중심 x = origin + k * w
        float origin = layer.localPosition.x + layer.scroll;

        // 타일이 화면과 겹치는 k 범위
        int kMin = Mathf.CeilToInt((left - layer.bounds.max.x - origin) / w);
        int kMax = Mathf.FloorToInt((right - layer.bounds.min.x - origin) / w);

        if (kMax - kMin + 1 > MaxTilesPerLayer)
        {
            kMax = kMin + MaxTilesPerLayer - 1;
        }

        for (int k = kMin; k <= kMax; k++)
        {
            var pos = new Vector3(origin + k * w, layer.localPosition.y, layer.localPosition.z);
            Matrix4x4 m = parentToWorld * Matrix4x4.Translate(pos);

            Graphics.RenderSprite(layer.renderParams, layer.spriteParams, 0, m);
        }
    }
}