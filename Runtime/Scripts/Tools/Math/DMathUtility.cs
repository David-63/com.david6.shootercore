using UnityEngine;

namespace David6.ShooterCore.Tools
{
    public static class DMathUtility
    {
        public static void GetOrthonormalBasis(Vector3 forward, out Vector3 right, out Vector3 up)
        {
            Vector3 worldUp = Vector3.up;
            right = Vector3.Cross(worldUp, forward);

            if (right.sqrMagnitude < 1e-6f)
                right = Vector3.Cross(Vector3.forward, forward);

            right.Normalize();
            up = Vector3.Cross(forward, right).normalized;
        }
    }
}