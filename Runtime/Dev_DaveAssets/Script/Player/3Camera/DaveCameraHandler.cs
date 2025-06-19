using System.Collections.Generic;
using Dave6.ShooterFramework.Provider;
using UnityEngine;


namespace Dave6.ShooterFramework.Camera
{
    public class DaveCameraHandler : MonoBehaviour, ICameraInfoProvider
    {
        [SerializeField] private List<Transform> ModeOfCamera;
        [SerializeField] private Transform _MainCamera;
        public float YawAngle => _MainCamera.eulerAngles.y;
    }
}
