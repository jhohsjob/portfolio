using UnityEngine;


public static class CameraFrustum
{
    private static Plane[] _planes;

    public static Plane[] planes => _planes;

    public static void Update(Camera camera)
    {
        _planes = GeometryUtility.CalculateFrustumPlanes(camera);
    }
}