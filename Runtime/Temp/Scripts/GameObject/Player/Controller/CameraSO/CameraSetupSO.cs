using UnityEngine;

namespace David6.ShooterFramework
{    
    [CreateAssetMenu(fileName = "CameraSetupSO", menuName = "Scriptable Objects/CameraSetupSO")]
    public class CameraSetupSO : ScriptableObject
    {
        public float FieldOfView = 75;
        public float CameraDistance = 0.8f;
    }
}