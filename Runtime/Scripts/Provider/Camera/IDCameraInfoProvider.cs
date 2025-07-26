using UnityEngine;

namespace David6.ShooterCore.Provider
{
    /// <summary>
    /// 카메라 정보 제공자 인터페이스
    /// </summary>
    public interface IDCameraInfoProvider
    {
        Vector2 InputLook { get; }
        /// <summary>
        /// 카메라의 수평 회전 각도 제공
        /// </summary>
        float YawAngle { get; }

        /// <summary>
        /// 카메라가 따라갈 대상 설정, 외부에서는 값을 설정만 가능
        /// </summary>
        bool SetCameraHolder(GameObject cameraHolder);
        void HandleLookInput(Vector2 input);

    }
}