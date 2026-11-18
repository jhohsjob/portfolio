using UnityEngine;

public class PosWatcher : MonoBehaviour
{
    private Vector3 _lastLocal, _lastWorld;
    private Transform _lastParent;

    private void OnEnable() { Snapshot("enable"); }

    private void Snapshot(string tag)
    {
        _lastLocal = transform.localPosition;
        _lastWorld = transform.position;
        _lastParent = transform.parent;
    }

    private void OnTransformParentChanged() { Snapshot("PARENT CHANGED"); }

    private void LateUpdate()
    {
        if (transform.localPosition != _lastLocal || transform.parent != _lastParent)
        {
            Snapshot("CHANGED");
        }
    }
}