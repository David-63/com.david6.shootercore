using System;
using Dave6.ShooterFramework.Data;
using Dave6.ShooterFramework.Movement.StateMachine;
using Dave6.ShooterFramework.Provider;
using David6.ShooterFramework;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public partial class DaveMovementController : MonoBehaviour, ICharacterControllerProvider
    {
        #region 외부 변수
        public DaveMovementProfile MovementProfile;
        [SerializeField] private GameObject CameraHolder;
        #endregion

        #region 캐싱 변수
        // [외부에서 가져옴]
        private CharacterController _characterController;
        private ICameraInfoProvider _cameraInfo;

        // [속력 변수]
        public float _currentSpeed;
        // [회전 변수]
        private float _rotationSpeed;
        private float _targetRotation;
        private float _characterRotation;
        // [카메라 회전 변수]
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // [수직 이동 변수]
        private float _verticalSpeed;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // [상수]
        private const float _terminalVelocity = 53.0f; // 왜 53인지 모르겠음
        private const float _speedOffset = 0.1f;
        private const float _threshold = 0.01f;
        #endregion

        #region 인풋 캐싱
        //public Vector2 Move;
        public Vector2 Look;

        public bool Sprint;
        public bool Jump;
        #endregion


        #region 외부 전달 함수
        public CharacterController GetController() => _characterController;
        public bool Grounded { get; private set; }
        public float TargetSpeed { get; set; }  // State에서 설정
        public Vector3 InputDirection { get; private set; }
        public Vector3 LastDirection;
        public Vector3 MoveDirection;
        #endregion

        #region FSM
        private DaveMotionStateMachine _motionStateMachine;
        #endregion

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _motionStateMachine = new DaveMotionStateMachine(this);
        }

        private void Start()
        {
            gameObject.layer = 3;
            _characterController.stepOffset = 0.25f;
            _characterController.skinWidth = 0.02f;
            _characterController.minMoveDistance = 0;
            _characterController.center = new Vector3(0, 0.93f, 0);
            if (MovementProfile)
            {
                _characterController.radius = MovementProfile.GroundedRadius;
            }
            _characterController.height = 1.8f;
        }

        private void Update()
        {
            ApplyGravity();
            GroundCheck();

            _motionStateMachine.Update();

            ApplyCharacterRotation();
            ApplyMovement();
        }

        private void ApplyGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = MovementProfile.FallTimeout;

                if (_verticalSpeed < 0.0f)
                {
                    _verticalSpeed = -2f;
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = MovementProfile.JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
            }

            if (_verticalSpeed < _terminalVelocity)
            {
                _verticalSpeed += MovementProfile.Gravity * Time.deltaTime;
            }
        }
        private void GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - MovementProfile.GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, MovementProfile.GroundedRadius, MovementProfile.GroundLayers, QueryTriggerInteraction.Ignore);
        }
        

        private void ApplyCharacterRotation()
        {
            if (HasMovementInput())
            {
                _targetRotation = Mathf.Atan2(LastDirection.x, LastDirection.z) * Mathf.Rad2Deg + _cameraInfo.YawAngle;
                if (!IsForward()) _targetRotation += 180f;

                _characterRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationSpeed, MovementProfile.RotationSmoothTime);
            }

            transform.rotation = Quaternion.Euler(0f, _characterRotation, 0f);
        }

        private void ApplyMovement()
        {
            Vector3 velocity = MoveDirection * _currentSpeed + Vector3.up * _verticalSpeed;
            _characterController.Move(velocity * Time.deltaTime);
        }

        private void LateUpdate()
        {
            CameraRotation();
            ClearInput();
        }

        private void CameraRotation()
        {
            if (Look.sqrMagnitude >= _threshold && !MovementProfile.LockCameraPosition)
            {
                _cinemachineTargetYaw += Look.x;
                _cinemachineTargetPitch += Look.y;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, MovementProfile.BottomClamp, MovementProfile.TopClamp);

            CameraHolder.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + MovementProfile.CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void ClearInput()
        {
            if (Jump) Jump = false;

        }

        #region Input Reception
        public void SetCameraInfoProvider(ICameraInfoProvider cameraInfoProvider)
        {
            _cameraInfo = cameraInfoProvider;
        }
        public void HandleMoveInput(Vector2 moveInput)
        {
            InputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;
        }
        public void HandleLookInput(Vector2 lookInput)
        {
            Look = lookInput;
        }
        public void HandleJumpInput()
        {
            Jump = true;
        }
        public void HandleStartSprintInput()
        {
            Sprint = true;
        }
        public void HandleStopSprintInput()
        {
            Sprint = false;
        }
        #endregion

    }
}
// [Combat Provider 필요함]
            // if (!_inputAim)
            // {
            //     if (HasMovementInput())
            //     {
            //         _targetRotation = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + cameraAngle;
            //         if (!IsForward()) _targetRotation += 180f;

            //         _characterRotation = Mathf.SmoothDampAngle(
            //             transform.eulerAngles.y, _targetRotation,
            //             ref _rotationVelocity, MovementProfile.RotationSmoothTime);
            //     }
            // }
            // else
            // {
            //      [Focus 상태인 경우]
            //      _characterRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, cameraAngle, ref _rotationVelocity, MovementProfile.RotationSmoothTime);
            //      transform.rotation = Quaternion.Euler(0f, _characterRotation, 0f);
            // }