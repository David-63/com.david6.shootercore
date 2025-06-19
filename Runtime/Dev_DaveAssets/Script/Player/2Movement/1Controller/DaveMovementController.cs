using System;
using Dave6.ShooterFramework.Data;
using Dave6.ShooterFramework.Movement.StateMachine;
using Dave6.ShooterFramework.Provider;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class DaveMovementController : MonoBehaviour, ICharacterControllerProvider
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
        private float _currentSpeed;
        // [회전 변수]
        private float _rotationSpeed;
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
        public Vector2 Move;
        public Vector2 Look;

        public bool Sprint;
        public bool Jump;
        #endregion


        #region 외부 전달 함수
        public CharacterController GetController() => _characterController;
        public bool Grounded { get; private set; }
        public float CurrentSpeed => _currentSpeed;
        public float TargetSpeed { get; set; }  // State에서 설정
        public Vector3 InputDirection { get; private set; }
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
            ReadInput();

            ApplyGravity();
            GroundCheck();

            _motionStateMachine.Update();

            ClearInput();
        }


        private void ReadInput()
        {
            InputDirection = new Vector3(Move.x, 0.0f, Move.y).normalized;
        }
        private void ApplyGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = MovementProfile.FallTimeout;

                // _isJump = false;
                // _isFreeFall = false;

                // stop our velocity dropping infinitely when grounded
                if (_verticalSpeed < 0.0f)
                {
                    _verticalSpeed = -2f;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = MovementProfile.JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    //_isFreeFall = true;
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
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

        public void CalculateMoveSpeed()
        {
            float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

            if (Mathf.Abs(currentHorizontalSpeed - TargetSpeed) > _speedOffset)
            {
                float desired = TargetSpeed;

                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, desired, Time.deltaTime * MovementProfile.SpeedChangeRate);
                // round speed to 3 decimal places
                _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
            }
            else
            {
                _currentSpeed = TargetSpeed;
            }
        }
        public void MoveWithRotation()
        {
            if (InputDirection.sqrMagnitude == 0f) return;

            // 회전
            float targetRotation = Mathf.Atan2(InputDirection.x, InputDirection.z) * Mathf.Rad2Deg + _cameraInfo.YawAngle;
            float smoothed = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref _rotationSpeed, MovementProfile.RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothed, 0f);

            // 이동
            Vector3 moveDir = Quaternion.Euler(0f, _cameraInfo.YawAngle, 0f) * InputDirection;
            Vector3 velocity = moveDir.normalized * CurrentSpeed + Vector3.up * _verticalSpeed;
            _characterController.Move(velocity * Time.deltaTime);
        }
        private void ClearInput()
        {
            Jump = false;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void CameraRotation()
        {
            // if (!_cursorLocked)
            // {
            //     _axisLook = Vector3.zero;
            // }

            // if there is an input and camera position is not fixed
            if (Look.sqrMagnitude >= _threshold && !MovementProfile.LockCameraPosition)
            {
                // 아날로그 방식을 사용하는 경우, Time.deltaTime을 곱해줘야함
                _cinemachineTargetYaw += Look.x;
                _cinemachineTargetPitch += Look.y;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, MovementProfile.BottomClamp, MovementProfile.TopClamp);

            // Cinemachine will follow this target
            CameraHolder.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + MovementProfile.CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }



        #region Input Reception
        public void SetCameraInfoProvider(ICameraInfoProvider cameraInfoProvider)
        {
            _cameraInfo = cameraInfoProvider;
        }
        public void HandleMoveInput(Vector2 moveInput)
        {
            Move = moveInput;
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

        private void InputPressReset()
        {
            Jump = false;
        }
        #endregion

        #region 외부 제어 함수

        #endregion

        #region 유틸 함수
        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        #endregion
    }
}
