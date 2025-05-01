using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;


namespace David6.ShooterFramework
{
    public class DavidCharacterCamera : MonoBehaviour
    {
        [Header("Framing")]
        public Camera Camera;
        public Vector2 FollowPointFraming = new Vector2(0f, 0f);
        public float FollowingSharpness = 10000f;

        [Header("Distance")]
        public float CameraDefaultDistance = 6f;
        public float CameraMinDistance = 0f;
        public float CameraMaxDistance = 10f;
        public float CameraDistanceMovementSpeed = 5f;
        public float CameraDistanceMovementSharpness = 10f;

        [Header("Rotation")]
        public bool InvertX = false;
        public bool InvertY = false;
        [Range(-90f, 90f)]
        public float DefaultVerticalAngle = 20f;
        [Range(-90f, 90f)]
        public float MinVerticalAngle = -90f;
        [Range(-90f, 90f)]
        public float MaxVerticalAngle = 90f;
        public float RotationSpeed = 1f;
        public float RotationSharpness = 10000f;
        public bool RotateWithPhysicsMover = false;

        [Header("Obstruction")]
        public float ObstructionCheckRadius = 0.2f;
        public LayerMask ObstructionLayers = -1;
        public float ObstructionSharpness = 10000f;


        [Header("Crosshair")]
        public DecalProjector CrosshairProjector;
        public LayerMask HitMask;
        public float MaxRange = 75f;
        
        public Transform Transform { get; private set; }
        public Transform FollowRootTransform { get; private set; }
        public Transform FollowHeadTransform { get; private set; }

        public Vector3 PlanarDirection { get; set; }
        public float TargetDistance { get; set; }

        private float _currentDistance;
        private float _targetVerticalAngle;
        private RaycastHit _obstructionHit;
        private int _obstructionCount;
        private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
        private float _obstructionTime;
        private Vector3 _currentFollowPosition;
        private List<Collider> _ignoredColliders = new List<Collider>();


        private const int MaxObstructions = 32;

        void OnValidate()
        {
            CameraDefaultDistance = Mathf.Clamp(CameraDefaultDistance, CameraMinDistance, CameraMaxDistance);
            DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
        }

        void Awake()
        {
            Transform = this.transform;

            _currentDistance = CameraDefaultDistance;
            TargetDistance = _currentDistance;

            _targetVerticalAngle = 0f;

            PlanarDirection = Vector3.forward;
        }

        void Update()
        {
            UpdateCrosshairScale();
        }

        // Set the transform that the camera will orbit around
        public void SetFollowTransform(Transform root, Transform head)
        {
            FollowRootTransform = root;
            FollowHeadTransform = head;
            PlanarDirection = FollowRootTransform.forward;

            _currentFollowPosition = FollowHeadTransform.position;
        }
        public void SetIgnoredColliders(List<Collider> colliders)
        {
            _ignoredColliders.Clear();
            _ignoredColliders = colliders;
        }

        public void UpdateWithInput(float deltaTime, Vector3 rotationInput)
        {
            if (FollowRootTransform)
            {
                if (InvertX)
                {
                    rotationInput.x *= -1f;
                }
                if (InvertY)
                {
                    rotationInput.y *= -1f;
                }

                // 목표 회전값
                Quaternion targetRotation = UpdateRotation(deltaTime, rotationInput);                

                // 카메라 목표 위치를 보간해서 가져옴
                _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowHeadTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

                HandleObstructions(deltaTime);

                // 카메라 배치
                Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

                // 오프셋으로 카메라 위치 보정?
                targetPosition += Transform.right * FollowPointFraming.x;
                targetPosition += Transform.up * FollowPointFraming.y;

                // 최종 위치 적용
                Transform.position = targetPosition;
            }
        }

        /// <summary>
        /// 장애물 충돌 처리
        /// </summary>
        /// <param name="deltaTime">Player로부터 전달받은 델타타임</param>
        private void HandleObstructions(float deltaTime)
        {
            RaycastHit closestHit = new RaycastHit();
            closestHit.distance = Mathf.Infinity;
            _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < _obstructionCount; i++)
            {
                // 무시처리
                bool isIgnored = false;
                for (int j = 0; j < _ignoredColliders.Count; j++)
                {
                    if (_ignoredColliders[j] == _obstructions[i].collider)
                    {
                        isIgnored = true;
                        break;
                    }
                }
                for (int j = 0; j < _ignoredColliders.Count; j++)
                {
                    if (_ignoredColliders[j] == _obstructions[i].collider)
                    {
                        isIgnored = true;
                        break;
                    }
                }

                // 가장 가까운 장애물 결정
                if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                {
                    closestHit = _obstructions[i];
                }
            }

            // 장애물 판별시: 거리 조정
            if (closestHit.distance < Mathf.Infinity)
            {
                _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
            }
            // 장애물 없는 경우: 목표 거리로 보간
            else
            {
                _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-CameraDistanceMovementSharpness * deltaTime));
            }
        }

        /// <summary>
        /// 입력 회전처리
        /// </summary>
        /// <param name="deltaTime">Player로부터 전달받은 델타타임</param>
        /// <param name="rotationInput">마우스 입력</param>
        /// <returns></returns>
        private Quaternion UpdateRotation(float deltaTime, Vector3 rotationInput)
        {
            // // 수평(yaw) 회전 업데이트
            // float yawDegree = rotationInput.x * RotationSpeed * deltaTime;
            // Quaternion yawDelta = Quaternion.Euler(yawDegree * FollowTransform.up);
            // PlanarDirection = yawDelta * PlanarDirection;
            // // 추가 보정
            // PlanarDirection = Vector3.ProjectOnPlane(PlanarDirection, FollowTransform.up).normalized;
            // Quaternion planarRotation = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

            // // 수직(pitch) 회전 업데이트
            // _targetVerticalAngle -= rotationInput.x * RotationSpeed * deltaTime;
            // _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
            // Quaternion pitchRotation = Quaternion.Euler(_targetVerticalAngle, 0f, 0f);

            // // 두 회전을 합친 목표 회전과 현재 회전의 보간
            // Quaternion targetRotation = planarRotation * pitchRotation;
            // Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

            // // 적용
            // transform.rotation = smoothRotation;
            // return smoothRotation;




            //============================================================================
            // 수평 회전
            Quaternion rotationFromInput = Quaternion.Euler(FollowRootTransform.up * (rotationInput.x * RotationSpeed));
            PlanarDirection = rotationFromInput * PlanarDirection;  // 입력 업데이트
            PlanarDirection = Vector3.Cross(FollowRootTransform.up, Vector3.Cross(PlanarDirection, FollowRootTransform.up)); // 수평 평면으로 투영, 이중 외적을 통해 Follow.Up 벡터에 수직인 평면에 투영하여 수평 회전에 집중함
            Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowRootTransform.up); // 수평 회전의 쿼터니언 생성

            // 수직 회전
            _targetVerticalAngle -= (rotationInput.y * RotationSpeed); // 회전 누적
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle); // 각도 제한
            Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0); // 수직 회전 생성
            Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime)); // 수평과 수직을 결합한 후, 현재 회전과 목표 회전간에 slerp 전환을 적용

            // 결합한 회전값 적용
            Transform.rotation = targetRotation;

            return targetRotation;
        }

        void UpdateCrosshairScale()
        {
            Ray ray = Camera.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, MaxRange, HitMask))
            {
                CrosshairProjector.gameObject.SetActive(true);
                CrosshairProjector.transform.SetPositionAndRotation(hit.point, transform.rotation);
            }
            else
            {
                CrosshairProjector.gameObject.SetActive(false);
                
            }
        }
    }    
}


