
using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.Camera
{
    public class DCameraHandler : MonoBehaviour, IDCameraInfoProvider
    {
        /// <summary>
        /// 메인 카메라의 Transform
        /// </summary>
        public Transform MainCamera;

        /// <summary>
        /// 카메라가 따라갈 GameObject
        /// </summary>
        public GameObject CameraHolder { get; set; }

        // ScriptableObject로 설정할 수 있는 카메라 프로필
        [Header("Camera Profile")]
        [Tooltip("카메라의 시점 프로필")]
        public DCameraLookProfile CameraLookProfile;        

        /// <summary>
        /// 카메라 입력 값을 캐싱하는 변수
        /// </summary>
        public Vector2 Look;


        private float _cameraYaw = 0.0f;
        private float _cameraPitch = 0.0f;
        private const float _threshold = 0.01f; // 카메라 회전 임계값


        
        /// <summary>
        /// Yaw 각도
        /// </summary>
        public float YawAngle => MainCamera.eulerAngles.y;

        void LateUpdate()
        {
            LookRotation();
        }

        void LookRotation()
        {
            // Look 벡터가 임계값 이상일 때만 카메라 회전 적용
            if (Look.sqrMagnitude >= _threshold)
            {
                _cameraYaw += Look.x;
                _cameraPitch += Look.y;
            }

            _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
            _cameraPitch = ClampAngle(_cameraPitch, CameraLookProfile.BottomClamp, CameraLookProfile.TopClamp);

            CameraHolder.transform.rotation = Quaternion.Euler(_cameraPitch + CameraLookProfile.CameraAngleOverride, _cameraYaw, 0.0f);
        }


        public void HandleLookInput(Vector2 lookInput)
        {
            Look = lookInput;
        }
        static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}