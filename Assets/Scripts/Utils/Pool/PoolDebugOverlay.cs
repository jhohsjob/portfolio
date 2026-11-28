#if DEBUG_MODE
using UnityEngine;


public class PoolDebugOverlay : MonoBehaviour
{
    private bool _visible;

    private void OnGUI()
    {
        float scale = Screen.height / 1080f;
        GUI.matrix = Matrix4x4.Scale(Vector3.one * Mathf.Max(1f, scale));

        if (GUI.Button(new Rect(10, 10, 80, 40), "POOL"))
        {
            _visible = !_visible;
        }

        if (_visible == false)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(10, 55, 460, 600), GUI.skin.box);
        foreach (var p in PoolRegistry.pools)
        {
            GUILayout.Label($"{p.name}  active {p.countActive} / pooled {p.countInactive} / total {p.countAll} (peak {p.peakActive})");
        }
        GUILayout.EndArea();
    }
}
#endif