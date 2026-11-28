using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class FlashShader : MonoBehaviour
{
    [SerializeField]
    private float _flashDuration = 0.2f;
    [SerializeField]
    private Color _flashColor = Color.white;

    private SpriteRenderer _sr;
    private MaterialPropertyBlock _mpb;

    private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColorID = Shader.PropertyToID("_FlashColor");

    private Coroutine _flashRoutine;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _mpb = new MaterialPropertyBlock();

        _sr.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashAmountID, 0f);
        _mpb.SetColor(FlashColorID, _flashColor);
        _sr.SetPropertyBlock(_mpb);
    }

    public void FlashOnce()
    {
        if (_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }
        _flashRoutine = StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        float t = 0f;
        while (t < _flashDuration)
        {
            t += Time.deltaTime;
            float ratio = 1f - (t / _flashDuration);
            _sr.GetPropertyBlock(_mpb);
            _mpb.SetFloat(FlashAmountID, ratio);
            _sr.SetPropertyBlock(_mpb);
            yield return null;
        }
        _sr.GetPropertyBlock(_mpb);
        _mpb.SetFloat(FlashAmountID, 0f);
        _sr.SetPropertyBlock(_mpb);
        _flashRoutine = null;
    }
}