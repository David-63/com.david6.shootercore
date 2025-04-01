using UnityEngine;
using UnityEngine.InputSystem;

namespace David6.ShooterFramework
{
    [RequireComponent(typeof(CharacterController))]
    public class Movement : MovementBehaviour
    {

        #region Fields

		public bool _grounded = true;
        /// <summary>
        /// 거친 지형에서 확인하는데 유용함.
        /// </summary>
        private float _groundedOffset = -0.14f;
        /// <summary>
        /// PlayerController 의 반경과 일치해야함.
        /// </summary>
        private float _groundedRadius = 0.5f;
        /// <summary>
        /// 다시 점프하는데 걸리는 시간.
        /// </summary>
        private float _jumpTimeout = 0.20f;
        /// <summary>
        /// 낙하 상태에 진입하는 시간. 계단을 내려갈때 유용함.
        /// </summary>
        private float _fallTimeout = 0.15f;
		/// <summary>
        /// 캐릭터에 적용되는 중력값. 엔진 기본값은 -9.81f.
        /// </summary>
		private float _gravity = -15.0f;


        // cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _applySpeed;
		private float _rotationVelocity;
		public float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

        private IPlayerManager _playerManager;
        private CharacterBehaviour _playerCharacter;
		private CharacterController _controller;
		private PlayerInput _playerInput;

		private const float _threshold = 0.01f;

        #endregion


        #region 유니티 기본 함수

        protected override void Awake()
        {
            _playerManager = ServiceLocator.Current.Get<IPlayerManager>();
            _playerCharacter = _playerManager.GetPlayerCharacter();
        }
        protected override void Start()
        {
            _controller = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();

            _jumpTimeoutDelta = _jumpTimeout;
			_fallTimeoutDelta = _fallTimeout;

        }
        protected override void Update()
        {
            ApplyGravity();
			GroundedCheck();
			HandleMovement();
        }
        protected override void LateUpdate()
        {
            //CameraRotation();
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

        private bool IsCurrentDeviceMouse
		{
			get
			{
				return _playerInput.currentControlScheme == "KeyboardMouse";
			}
		}

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _playerManager.GetGroundLayer(), QueryTriggerInteraction.Ignore);
        }

        #endregion

        #region 로직 함수

        private void ApplyGravity()
        {
            if (_grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = _fallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_playerCharacter.IsJumping() && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(_playerManager.GetJumpHeight() * -2f * _gravity);
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
				_jumpTimeoutDelta = _jumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += _gravity * Time.deltaTime;
			}            
        }

        private void HandleMovement()
        {
            #region 목표 속력 구하기
            // 속력 구하고
            float targetSpeed = _playerCharacter.IsSprinting() ? _playerManager.GetSprintSpeed() : _playerManager.GetWalkSpeed();
            Vector2 playerInput = _playerCharacter.GetInputMovement();
            
            // 입력이 없으면 목표속도를 0 으로 설정
            if (playerInput == Vector2.zero) targetSpeed = 0.0f;

            // 플레이어의 현재 수평속도를 가져옴
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            
            // 목표속력의 가속 및 감속 계산
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // 직선적인 결과 대신에 곡선적인 결과를 만들어내어 더욱 유기적인 속도변화를 제공
                _applySpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _playerManager.GetSpeedChangeRate());
                // 속력을 소숫점 3자리까지 반올림
                _applySpeed = Mathf.Round(_applySpeed * 1000f) / 1000f;
            }
            else
            {
                _applySpeed = targetSpeed;
            }

            #endregion

            #region 입력 방향 구하기

            Vector3 inputDirection = new Vector3(playerInput.x, 0.0f, playerInput.y);
            if (playerInput != Vector2.zero)
            {
                inputDirection = transform.right * playerInput.x + transform.forward * playerInput.y;
            }

            #endregion

            Vector3 applyHorizontalVelocity = inputDirection.normalized * (_applySpeed * Time.deltaTime);
            Vector3 applyVerticalVelocity = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
            _controller.Move(applyHorizontalVelocity + applyVerticalVelocity);
        }

        private void JumpCharacter()
        {
            // 플레이어의 입력이 있고, 점프가 가능한 상태 (GroundCheck 단계에서 점프 가능여부 확인)
            
        }
        private void DodgeCharacter()
        {
            // 플레이어의 입력이 있고, 회피가 가능한 상태(아마도 IEnumerator으로 쿨다운 세팅할듯?)

        }

        private void CameraRotation()
		{
			// if there is an input
			if (_playerCharacter.GetInputLook().sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _playerCharacter.GetInputLook().y * _playerManager.GetRotationSpeed() * deltaTimeMultiplier;
				_rotationVelocity = _playerCharacter.GetInputLook().x * _playerManager.GetRotationSpeed() * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _playerManager.GetBottomClamp(), _playerManager.GetTopClamp());

				// Update Cinemachine camera target pitch
                _playerManager.GetCinemachineCameraTarget().transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

        #endregion

    }
}
