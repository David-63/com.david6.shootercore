using System.Collections.Generic;
using System.Linq;
using David6.ShooterCore.Context;
using David6.ShooterCore.Data;
using David6.ShooterCore.Provider;
using David6.ShooterCore.TickSystem;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Look
{
    public enum EDCameraType
    {
        None = -1,
        Exploration,
        Focus,
        Aim,
        Pause,
    }
    public enum EDCameraLayer
    {
        Exploration = 0,
        Focus = 10,
        Aim = 15,
        Pause = 50,
    }
    public class DCameraHandler : MonoBehaviour, IDCameraHandlerProvider, IDLateTickable
    {
        // ScriptableObject로 설정할 수 있는 카메라 프로필
        [Tooltip("Camera Profile")]
        [SerializeField] DCameraLookProfile CameraLookProfile;
        [Header("Camera Prefabs")]
        [SerializeField] GameObject ExplorationCamera;
        [SerializeField] GameObject FocusCamera;
        [SerializeField] GameObject AimCamera;
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

        Camera _lookCamera;
        public Camera LookCamera => _lookCamera;
        public LayerMask HitMask;


        // 카메라 종류 관리
        Dictionary<EDCameraType, GameObject> _cameraMap;
        Dictionary<EDCameraLayer, bool> _cameraLayer = new();
        EDCameraLayer _currentCameraLayer = EDCameraLayer.Exploration;

        void Awake()
        {
            var bootstrapper = GetComponent<DPlayerBootstrapper>();
            bootstrapper.Register<IDCameraHandlerProvider>(this);

            _cameraMap = new Dictionary<EDCameraType, GameObject>()
            {
                { EDCameraType.Exploration, ExplorationCamera },
                { EDCameraType.Focus, FocusCamera },
                { EDCameraType.Aim, AimCamera },
                { EDCameraType.Pause, MenuCamera }
            };

            foreach (EDCameraLayer layer in System.Enum.GetValues(typeof(EDCameraLayer)))
            {
                _cameraLayer[layer] = false;
            }
            _cameraLayer[EDCameraLayer.Exploration] = true;
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

            AimFollow();
        }

        private void AimFollow()
        {
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


        public void ActivateCamera(EDCameraType type)
        {
            foreach (var kvp in _cameraMap)
            {
                bool active = kvp.Key == type;
                kvp.Value.SetActive(active);
            }
        }

        public void SetLayerActive(EDCameraLayer layer, bool active)
        {
            if (_cameraLayer.ContainsKey(layer))
            {
                _cameraLayer[layer] = active;
                CameraUpdate();
            }
        }

        void CameraUpdate()
        {
            // 카메라 우선순위 계산
            var highest = _cameraLayer.Where(kvp => kvp.Value)
                .OrderByDescending(kvp => kvp.Key)
                .Select(kvp => kvp.Key).FirstOrDefault();

            if (highest != _currentCameraLayer)
            {
                _currentCameraLayer = highest;
                SwitchCamera(_currentCameraLayer);

            }
        }

        void SwitchCamera(EDCameraLayer cameraLayer)
        {
            foreach (var kvp in _cameraMap)
            {
                bool active = kvp.Key == ConvertToType(cameraLayer);
                kvp.Value.SetActive(active);
            }
        }

        EDCameraType ConvertToType(EDCameraLayer cameraLayer)
        {
            return cameraLayer switch
            {
                EDCameraLayer.Exploration => EDCameraType.Exploration,
                EDCameraLayer.Focus => EDCameraType.Focus,
                EDCameraLayer.Aim => EDCameraType.Aim,
                EDCameraLayer.Pause => EDCameraType.Pause,
                _ => EDCameraType.Exploration
            };
        }
    }
}