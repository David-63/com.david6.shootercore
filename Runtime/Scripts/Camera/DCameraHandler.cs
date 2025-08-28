using System.Collections.Generic;
using David6.ShooterCore.Context;
using David6.ShooterCore.Data;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Provider;
using David6.ShooterCore.TickSystem;
using David6.ShooterCore.Tools;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace David6.ShooterCore.Look
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
        [SerializeField] GameObject AimTarget;


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
        float _cameraYaw = 0.0f;
        float _cameraPitch = 0.0f;
        const float _threshold = 0.01f; // 카메라 회전 임계값

        Dictionary<EDCameraType, GameObject> _cameraMap;
        Camera _lookCamera;
        public Camera LookCamera => _lookCamera;
        public LayerMask HitMask;

        void Awake()
        {
            var bootstrapper = GetComponent<DPlayerBootstrapper>();
            bootstrapper.Register<IDCameraHandlerProvider>(this);

            _cameraMap = new Dictionary<EDCameraType, GameObject>()
            {
                { EDCameraType.Exploration, ExplorationCamera },
                { EDCameraType.Focus, FocusCamera },
                { EDCameraType.Pause, MenuCamera }
            };
        }

        void Start()
        {
            DGameLoop.Instance.Register(this);
            _lookCamera = Camera.main;
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

            // RaycastHit 구하는 로직 추가
            // IK에 사용할 예정

            Ray ray = _lookCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));            

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, CameraLookProfile.MaxLookRange, HitMask))
            {
                AimTarget.transform.position = hit.point;
            }
            else
            {
                AimTarget.transform.position = _lookCamera.transform.position + _lookCamera.transform.forward * CameraLookProfile.MaxLookRange;
            }
        }

        public void ActivateCamera(EDCameraType type)
        {
            foreach (var kvp in _cameraMap)
            {
                bool active = kvp.Key == type;
                kvp.Value.SetActive(active);
            }
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