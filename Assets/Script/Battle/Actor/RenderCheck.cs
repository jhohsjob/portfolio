using System;
using UnityEngine;


public class RenderCheck : MonoBehaviour
{
    private Renderer _renderer;
    private bool _isVisible;

    public event Action<bool> onVisibleChanged;

    private void LateUpdate()
    {
        bool visible = false;
        if (_renderer != null)
        {
            visible = GeometryUtility.TestPlanesAABB(CameraFrustum.planes, _renderer.bounds);
        }
        SetVisible(visible);
    }

    public void SetRenderer(Renderer renderer)
    {
        _renderer = renderer;
    }

    private void SetVisible(bool visible)
    {
        if (_isVisible == visible)
        {
            return;
        }

        _isVisible = visible;

        onVisibleChanged?.Invoke(visible);
    }
}