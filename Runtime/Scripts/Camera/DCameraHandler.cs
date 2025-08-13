using David6.ShooterCore.Data;
using David6.ShooterCore.Provider;
using David6.ShooterCore.TickSystem;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Camera
{
    
    public class DCameraHandler : MonoBehaviour, IDCameraHandlerProvider, IDLateTickable
    {
        // ScriptableObject로 설정할 수 있는 카메라 프로필        
        [Tooltip("Camera Profile")]
        [SerializeField] DCameraLookProfile CameraLookProfile;
        [Header("Camera Prefabs")]
        [SerializeField] GameObject ExplorationCamera;
        [SerializeField] GameObject FocusCamera;
        [SerializeField] GameObject MenuCamera;

        GameObject _prevCamera;

        /// <summary>
        /// 메인 카메라의 Transform
        /// </summary>
        public Transform MainCamera;
        public float YawAngle => MainCamera.eulerAngles.y;

        /// <summary>
        /// 카메라가 따라갈 GameObject
        /// </summary>
        GameObject _targetCameraHolder;

        public Vector2 InputLook { get; private set; }

        // [카메라 transform 회전값]
        private float _cameraYaw = 0.0f;
        private float _cameraPitch = 0.0f;
        private const float _threshold = 0.01f; // 카메라 회전 임계값

        void Start()
        {
            DGameLoop.Instance.Register(this);
        }

        void OnDestroy()
        {
            DGameLoop.Instance.Unregister(this);
        }

        public bool SetCameraHolder(GameObject cameraHolder)
        {
            bool flag = true;
            if (cameraHolder != null)
            {
                _targetCameraHolder = cameraHolder;
            }
            else
            {
                flag = false;
            }

            return flag;
        }


        public void HandleLookInput(Vector2 input)
        {
            InputLook = input;
        }

        public void LateTick(float deltaTime)
        {
            LookRotation();
        }

        public void ActiveExplorationCamera()
        {
            if (_prevCamera != null) _prevCamera.SetActive(false);
            ExplorationCamera.SetActive(true);
            _prevCamera = ExplorationCamera;
        }
        public void ActiveFocusCamera()
        {
            if (_prevCamera != null) _prevCamera.SetActive(false);
            FocusCamera.SetActive(true);
            _prevCamera = FocusCamera;
        }
        public void ActiveMenuCamera()
        {
            if (_prevCamera != null) _prevCamera.SetActive(false);
            MenuCamera.SetActive(true);
        }
        public void InactiveMenuCamera()
        {
            MenuCamera.SetActive(false);
            _prevCamera.SetActive(true);
        }

        void LookRotation()
        {
            // Look 벡터가 임계값 이상일 때만 카메라 회전 적용
            if (InputLook.sqrMagnitude >= _threshold)
            {
                _cameraYaw += InputLook.x;
                _cameraPitch += InputLook.y;
            }

            _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
            _cameraPitch = ClampAngle(_cameraPitch, CameraLookProfile.BottomClamp, CameraLookProfile.TopClamp);

            _targetCameraHolder.transform.rotation = Quaternion.Euler(_cameraPitch + CameraLookProfile.CameraAngleOverride, _cameraYaw, 0.0f);
        }


        static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}