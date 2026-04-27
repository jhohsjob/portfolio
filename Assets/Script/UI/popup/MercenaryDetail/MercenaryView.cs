using UnityEngine;

public class MercenaryView : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetLocked(bool isLocked)
    {
        _spriteRenderer.color = isLocked ? Color.black : Color.white;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void ResetView()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white;
        }
        gameObject.SetActive(false);
    }
}