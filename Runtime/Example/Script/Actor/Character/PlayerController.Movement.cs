using UnityEngine;

namespace David6.ShooterFramework
{
    public partial class PlayerController : ControllerBehaviour
    {
        #region Serialize fields

        [Header("Player Movmenet")]
        [Tooltip("How fast the character turns to face movement direction")][SerializeField]
        [Range(0.0f, 0.3f)]
		private float RotationSmoothTime = 0.12f;

        [Header("Player Grounded")]
        [Tooltip("캐릭터가 지면에 있는지 확인. CharacterController의 내장된 지면 체크 기능과는 별개임.")][SerializeField]
		private bool Grounded = true;

        [Tooltip("복잡한 지형에 유용함")][SerializeField]
        private float GroundedOffset = -0.14f;

		[Tooltip("지면 체크 범위. CharacterController의 범위와 일치해야함.")][SerializeField]
		private float GroundedRadius = 0.28f;

        [Space(10)]
        [Tooltip("캐릭터에 적용되는 중력값. 엔진 기본값은 -9.81f.")][SerializeField]
		private float Gravity = -15.0f;
        [Tooltip("다시 점프하는데 걸리는 시간.")][SerializeField]
		private float JumpTimeout = 0.50f;

		[Tooltip("낙하 상태에 진입하는 시간. 계단을 내려갈때 유용함.")][SerializeField]
		private float FallTimeout = 0.15f;

        #endregion

        #region Fields

        // ======
        // player
        // ======
        private float _applySpeed;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // =================
        // timeout deltatime
        // =================
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        
        #endregion

        #region 주요 로직 함수

        private void GroundedCheck()
        {
            // set sphere position, with offset
			var position = transform.position;
			Vector3 spherePosition = new Vector3(position.x, position.y - GroundedOffset, position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, _playerManager.GetGroundLayer(), QueryTriggerInteraction.Ignore);
        }

        private void HandleMovement()
        {
            #region 입력 및 속력 계산
            // 플레이어 입력 가져오기.
            Vector2 inputMovement = _playerCharacter.GetInputMovement();
            bool isInput = inputMovement != Vector2.zero;

            // 속력 초기값.
            float targetSpeed = 0.0f;

            
            if (isInput)
            {
                targetSpeed = _playerCharacter.IsSprinting() ? _playerManager.GetSprintSpeed() : _playerManager.GetWalkSpeed();
            }

            #endregion

            // 현재 수평 속도 가져오기.
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;

            // // 속력 공중제어
            // if (!Grounded)
            // {
            //     targetSpeed *= _playerManager.GetAirControlFactor();
            // }

            // 감속 혹은 가속하는 상황.
            if (Grounded)
            {
                if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
                {
                    // 속력 변환에 커브보간 적용 (소수점 3자리까지 적용).
                    _applySpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _playerManager.GetSpeedChangeRate());
                    _applySpeed = Mathf.Round(_applySpeed* 1000.0f) / 1000.0f;
                }
                else
                {
                    _applySpeed = targetSpeed;
                }
            }
            else
            {
                _applySpeed = currentHorizontalSpeed;
            }

            // 이동 입력이 있으면, 움직일 때 캐릭터를 회전시킴.
            if (isInput)
            {
                _targetRotation = Mathf.Atan2(inputMovement.x, inputMovement.y) * Mathf.Rad2Deg + _cameraTarget.eulerAngles.y;

                if (_firstPerson)
                {
                    _bodyRotate = Mathf.SmoothDampAngle(_bodyRotate, _cameraTarget.eulerAngles.y, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, _bodyRotate, 0.0f);
                }
                else
                {
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                    // 카메라의 위치에 따라 입력된 방향을 바라보도록 회전시킴.
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            

            // 캐릭터 이동시키기
            _controller.Move(targetDirection.normalized * (_applySpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void ApplyGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// update animator if using character


				// 지면에 있을 때 수직속도 멈춤
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_playerCharacter.IsJumping() && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(_playerManager.GetJumpHeight() * -2f * Gravity);

					// update animator if using character

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
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
				else
				{
					// update animator if using character

				}
			}

			// 터미널 속도에 미치지 못하면, 시간이 지남에 따라 중력을 적용합니다 (델타 타임을 두 번 곱해 시간이 지나면서 선형적으로 가속됩니다).
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

        #endregion

    }
}
