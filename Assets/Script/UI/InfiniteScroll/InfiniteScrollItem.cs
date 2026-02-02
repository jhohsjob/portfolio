using UnityEngine;

public struct ScrollOptions
{
    public bool useCenterScale;

    public ScrollOptions(bool useCenterScale)
    {
        this.useCenterScale = useCenterScale;
    }
}

public abstract class InfiniteScrollItem : MonoBehaviour
{
    [SerializeField]
    protected RectTransform _root;
    [SerializeField]
    float _scaleSmooth = 12f;

    public int index { get; private set; }
    
    protected bool _useCenterScale = false;
    protected float _currentScale = 1f;
    protected float _targetScale = 1f;

    protected InfiniteScrollBase _scroll;

    private void Update()
    {
        if (_useCenterScale == false)
        {
            return;
        }

        _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * _scaleSmooth);

        ApplyScale();
    }

    public virtual void SetData(int index, object data)
    {
        this.index = index;
    }

    public void Init(InfiniteScrollBase scroll)
    {
        _scroll = scroll;
        _useCenterScale = scroll.options.useCenterScale;
    }

    public virtual void OnRecycle()
    {
        _currentScale = 1f;

        if (_root == null)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            _root.localScale = Vector3.one;
        }
    }

    public virtual void SetScale(float scale)
    {
        _targetScale = scale;
    }

    private void ApplyScale()
    {
        if (_root == null)
        {
            transform.localScale = Vector3.one * _currentScale;
        }
        else
        {
            _root.localScale = Vector3.one * _currentScale;
        }
    }

    public void ForceSetScale(float scale)
    {
        _currentScale = scale;

        if (_root == null)
        {
            transform.localScale = Vector3.one * scale;
        }
        else
        {
            _root.localScale = Vector3.one * scale;
        }
    }
}