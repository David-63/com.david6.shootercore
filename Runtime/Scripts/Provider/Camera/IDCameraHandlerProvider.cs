using David6.ShooterCore.Data.Enum;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    /// <summary>
    /// 카메라 정보 제공자 인터페이스
    /// </summary>
    public interface IDCameraHandlerProvider : IDProvider
    {
        Vector2 InputLook { get; }
        float YawAngle { get; } // 카메라의 수평 회전 각도 제공

        bool SetCameraHolder(GameObject cameraHolder); // Follow Target
        void HandleLookInput(Vector2 input);
        void ActivateCamera(EDCameraType type);
    }
}