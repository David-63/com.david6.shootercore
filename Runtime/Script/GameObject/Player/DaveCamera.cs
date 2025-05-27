using Unity.Cinemachine;
using UnityEngine;

namespace David6.ShooterFramework
{
    public class DaveCamera : MonoBehaviour
    {
        public const float EngageFOV = 90;
        public const float DisengageFOV = 75;

        public const float ZoomIn = 0.4f;
        public const float ZoomOut = 0.8f;



        [Tooltip("레이의 최대 거리")]
        public float maxDistance = 100f;

        [Tooltip("레이 충돌을 검사할 레이어")]
        public LayerMask layerMask = Physics.DefaultRaycastLayers;

        public GameObject LookAtTarget;

        private CinemachineCamera _cinemachine;
        private CinemachineThirdPersonFollow _followComponent;

        public CameraSetupSO CurrentCameraSetup;

        void Start()
        {
            _cinemachine = GetComponent<CinemachineCamera>();
            _followComponent= GetComponent<CinemachineThirdPersonFollow>();
        }

        void Update()
        {
            UpdateCrosshair();
        }

        private void UpdateCrosshair()
        {
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;  // 카메라가 바라보는 방향 :contentReference[oaicite:0]{index=0}

            RaycastHit hitInfo;

            if (!LookAtTarget)
            {
                Log.AttentionPlease("크로스헤어 오브젝트가 없음.");
                return;
            }

            if (Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask))
            {
                LookAtTarget.gameObject.SetActive(true);
                LookAtTarget.transform.SetPositionAndRotation(hitInfo.point, transform.rotation);
            }
            else
            {
                LookAtTarget.gameObject.SetActive(false);
            }
            Debug.DrawRay(origin, direction * maxDistance, Color.green);
        }

        // TODO //

        // 1. fov 조절
        public void SetCameraFOV(bool engaged)
        {
            if (engaged)
            {
                _cinemachine.Lens.FieldOfView = EngageFOV;
            }
            else
            {
                _cinemachine.Lens.FieldOfView = DisengageFOV;
            }
        }

        // 2. zoom in out
        public void SetCameraDistance(bool zoom)
        {
            if (zoom)
            {
                _followComponent.CameraDistance = ZoomIn;
            }
            else
            {
                _followComponent.CameraDistance = ZoomOut;
            }
        }

        public void CameraSetup(CameraSetupSO setup)
        {
            CurrentCameraSetup = setup;
            _cinemachine.Lens.FieldOfView = setup.FieldOfView;
            _followComponent.CameraDistance = setup.CameraDistance;
        }

        // 3. 시점 변경 (나중에)

        // 4. Handedness 좌 우 변경 (나중에)




    }
}
