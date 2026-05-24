using System;
using UnityEngine;


public class BootController
{
    private const float _smoothSpeed = 3f;

    private float _current;
    private float _target;

    public event Action<float> onProgressChanged;

    public void SetProgress(float value)
    {
        _target = Mathf.Clamp01(value);
    }

    public void Update(float deltaTime)
    {
        if (_current >= _target)
        {
            return;
        }

        _current = Mathf.Lerp(_current, _target, deltaTime * _smoothSpeed);

        if (Mathf.Abs(_current - _target) < 0.001f)
        {
            _current = _target;
        }

        onProgressChanged?.Invoke(_current);
    }

    public void Complete()
    {
        _target = 1f;
    }
}
