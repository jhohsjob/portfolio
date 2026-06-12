using System.Collections;
using UnityEngine;


public class ActorDashView : ActorSubView
{
    private Material _material;

    public void SetMaterial(Material material)
    {
        _material = material;
    }

    public void PlayDashEffect(Vector2 direction, float duration)
    {
        if (_material == null || _renderer == null)
        {
            return;
        }

        StartCoroutine(CoDashEffect(direction, duration));
    }

    private IEnumerator CoDashEffect(Vector2 direction, float duration)
    {
        Material previous = _renderer.material;

        _renderer.material = _material;

        _material.SetVector("_Direction", new Vector4(direction.x, direction.y, 0f, 0f));
        _material.SetFloat("_BlurStrength", 1f);
        _material.SetFloat("_Alpha", 1f);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            _material.SetFloat("_BlurStrength", Mathf.Lerp(1f, 0f, t));
            _material.SetFloat("_Alpha", Mathf.Lerp(1f, 0f, t));

            yield return null;
        }

        _renderer.material = previous;
    }
}
