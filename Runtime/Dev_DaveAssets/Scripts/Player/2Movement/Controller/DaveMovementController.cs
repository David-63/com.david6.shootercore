using System;
using Dave6.ShooterFramework.Data;
using Dave6.ShooterFramework.Movement.StateMachine;
using Dave6.ShooterFramework.Provider;
using UnityEngine;


namespace Dave6.ShooterFramework.Movement
{
    public class DaveMovementController : MonoBehaviour, ICharacterControllerProvider
    {
        [Header("Assets")]
        [SerializeField] private DaveMovementProfile MovementProfile;
        [Tooltip("캐릭터 내부에 카메라가 참조할 Transform을 대상으로 함")]
        [SerializeField] private GameObject CameraHolder;

        private CharacterController _controller;
        private ICameraInfoProvider _cameraInfo;

        #region 이벤트 등록
        public event Action<AnimSpeedData> OnAnimSpeed;
        public event Action<AnimGroundData> OnAnimGround;
        public event Action<AnimInputDir> OnAnimDirection;

        private bool _isJump;
        private bool _isGrounded;
        private bool _isFreeFall;

        private Vector2 _currentInputDir;

        #endregion


        #region 입력 캐싱
        public Vector2 Move;
        public Vector2 Look;

        public bool Sprint;
        public bool Jump;
        #endregion

        #region 내부 변수
        // [속도 및 방향]
        private Vector3 _inputDirection;
        private float _currentSpeed;
        private float _targetSpeed;
        private float _targetRotation;


        /// <summary>
        /// 3인칭 캐릭터 회전
        /// </summary>
        private float _characterRotation;
        private float _rotationVelocity;

        private float _verticalVelocity;

        public bool _grounded;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // [카메라 회전 변수]
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        #endregion

        /// <summary>
        /// 왜 53인지 모름
        /// </summary>
        private const float _terminalVelocity = 53.0f;
        private const float _speedOffset = 0.1f;
        private const float _threshold = 0.01f;


        #region 외부 전달
        public Vector2 MoveInput => _currentInputDir;
        public CharacterController GetController() => _controller;
        #endregion

        private DaveMotionStateMachine _motionStateMachine;


        #region Movement Logic
        private void Awake()
        {
            // 캐릭터 오브젝트 세팅
            gameObject.layer = 3;
            _controller = GetComponent<CharacterController>();
            _controller.stepOffset = 0.25f;
            _controller.skinWidth = 0.02f;
            _controller.minMoveDistance = 0;
            _controller.center = new Vector3(0, 0.93f, 0);
            if (MovementProfile)
            {
                _controller.radius = MovementProfile.GroundedRadius;
            }
            _controller.height = 1.8f;

            _motionStateMachine = new DaveMotionStateMachine(this);
        }

        private void Start()
        {            
            
        }

        private void Update()
        {
            UpdateGravity();
            GroundedCheck();

            _motionStateMachine.Update();

            ReadInput();

            //Movement();

            // 가장 마지막에
            InputPressReset();

            //AnimDataFlush(); // 이건 state에서 진행

            void AnimDataFlush()
            {
                OnAnimSpeed?.Invoke(new AnimSpeedData(1f, _currentSpeed));
                OnAnimGround?.Invoke(new AnimGroundData(_isJump, _isGrounded, _isFreeFall));
                OnAnimDirection?.Invoke(new AnimInputDir(_currentInputDir.x, _currentInputDir.y));
            }
        }
        private void LateUpdate()
        {
            CameraRotation();
        }


        #region Input Reception
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

        public void SetCameraInfoProvider(ICameraInfoProvider cameraInfoProvider)
        {
            _cameraInfo = cameraInfoProvider;
        }
        #endregion


        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - MovementProfile.GroundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, MovementProfile.GroundedRadius, MovementProfile.GroundLayers, QueryTriggerInteraction.Ignore);

            _isGrounded = _grounded;
        }

        private void GroundedCheck_v2()
        {
            // set sphere position, with offset

            _grounded = _controller.isGrounded;
            _isGrounded = _grounded;

            // 지면 붙이기
            if (_isGrounded && _verticalVelocity < 0) _verticalVelocity = -2f;
        }

        private void PerformMove(Vector3 move)
        {
            Vector3 motion = move + _currentSpeed * Vector3.up;
            _controller.Move(motion * Time.deltaTime);
        }

        private void ApplyGravity()
        {
            _verticalVelocity += MovementProfile.Gravity * Time.deltaTime;
        }

        private void UpdateGravity()
        {
            if (_grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = MovementProfile.FallTimeout;

                _isJump = false;
                _isFreeFall = false;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
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
                    _isFreeFall = true;
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += MovementProfile.Gravity * Time.deltaTime;
            }
        }

        private void Movement()
        {

            CalculateSpeed();
            PerformMovement();

            if (CanJump())
            {
                PerformJump();
            }

            void CalculateSpeed()
            {
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                if (Mathf.Abs(currentHorizontalSpeed - _targetSpeed) > _speedOffset)
                {
                    float desired = _targetSpeed;

                    // creates curved result rather than a linear one giving a more organic speed change
                    // note T in Lerp is clamped, so we don't need to clamp our speed
                    _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, desired, Time.deltaTime * MovementProfile.SpeedChangeRate);
                    // round speed to 3 decimal places
                    _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
                }
                else
                {
                    _currentSpeed = _targetSpeed;
                }
            }

            void PerformJump()
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(MovementProfile.JumpHeight * -2f * MovementProfile.Gravity);

                _isJump = true;
            }
        }

        /// <summary>
        /// 폴링 입력은 여기서 처리
        /// </summary>
        private void ReadInput()
        {
            _inputDirection = new Vector3(Move.x, 0.0f, Move.y).normalized;

            if (HasMovementInput())
            {
                _targetSpeed = Sprint ? MovementProfile.SprintSpeed : MovementProfile.MoveSpeed;
            }
            else
            {
                _targetSpeed = 0.0f;
            }

            _currentInputDir = Move;
        }

        private void PerformMovement()
        {
            float cameraAngle = _cameraInfo.YawAngle;

            UpdateCharacterRotation(cameraAngle);
            ApplyMovement(cameraAngle);

            void UpdateCharacterRotation(float cameraAngle)
            {
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


                // 임시) 캐릭터가 인풋방향으로 회전
                if (HasMovementInput())
                {
                    _targetRotation = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + cameraAngle;
                    if (!IsForward()) _targetRotation += 180f;

                    _characterRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, MovementProfile.RotationSmoothTime);
                }

                // 최종 결정된 회전값 적용
                transform.rotation = Quaternion.Euler(0f, _characterRotation, 0f);
            }
            void ApplyMovement(float cameraAngle)
            {
                Vector3 moveDirection = Quaternion.Euler(0f, cameraAngle, 0f) * _inputDirection;
                moveDirection.Normalize();

                Vector3 velocity = moveDirection * _currentSpeed + Vector3.up * _verticalVelocity;
                _controller.Move(velocity * Time.deltaTime);
            }
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

        #endregion


        #region 헬퍼 메서드
        private bool HasMovementInput() => _inputDirection != Vector3.zero;
        private bool IsForward() => _currentInputDir.y >= -0.8f;

        private bool CanJump() => Jump && _jumpTimeoutDelta <= 0.0f ? true : false;

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            if (MovementProfile)
            {
                Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - MovementProfile.GroundedOffset, transform.position.z), MovementProfile.GroundedRadius);
            }
            else
            {
                Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - MovementProfile.GroundedOffset, transform.position.z), _controller.radius);
            }
        }
        #endregion

    }
}
