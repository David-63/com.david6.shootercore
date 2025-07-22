using UnityEngine;

namespace David6.ShooterCore.Data
{
    /// <summary>
    /// 카메라의 시점 프로필을 정의하는 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "CameraLookProfile", menuName = "Configs/Camera/CameraLookProfile")]
    public class DCameraLookProfile : ScriptableObject
    {
        [Header("Camera Settings")]
        [Tooltip("카메라를 얼마나 위로 올릴 수 있는지 (degree)")]
        public float TopClamp = 70.0f;
        [Tooltip("카메라를 얼마나 아래로 내릴 수 있는지 (degree)")]
        public float BottomClamp = -30.0f;
        [Tooltip("카메라 각도 오버라이드 (고정된 시점에서 조정용)")]
        public float CameraAngleOverride = 0.0f;
    }
}