using UnityEngine;


public static class CameraFrustum
{
    private static Plane[] _planes = new Plane[6];

    public static Plane[] planes => _planes;

    public static void Update(Camera camera)
    {
        GeometryUtility.CalculateFrustumPlanes(camera, _planes);
    }
}