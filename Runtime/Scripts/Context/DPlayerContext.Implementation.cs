using System.Collections;
using David6.ShooterCore.Provider;
using UnityEngine;

namespace David6.ShooterCore.Context
{
    /// <summary>
    /// Represents the player's movement component.
    /// </summary>
    public partial class DPlayerContext : MonoBehaviour, IDContextProvider
    {
        #region Input caching
        public Vector3 InputDirection { get; private set; }
        public Vector2 InputLook { get; private set; }
        public bool InputJump { get; private set; }
        public bool JumpInputCheck;
        public bool InputSprint { get; private set; }

        public bool HasMovementInput { get; private set; }
        public void HandleMoveInput(Vector2 moveInput)
        {
            // Process the move input and update InputDirection accordingly.
            InputDirection = new Vector3(moveInput.x, 0, moveInput.y);

            HasMovementInput = moveInput.x != 0 || moveInput.y != 0;
            // Additional logic for movement can be added here.
        }
        public void HandleLookInput(Vector2 lookInput)
        {
            // Process the look input and update Look accordingly.
            InputLook = lookInput;
            // Additional logic for camera rotation can be added here.
        }
        public void HandleStartJumpInput()
        {
            InputJump = true;
            JumpInputCheck = true;
        }
        public void HandleStopJumpInput()
        {
            InputJump = false;
            JumpInputCheck = false;
        }
        public void HandleStartSprintInput()
        {
            // Process the start sprint input.
            InputSprint = true;
            // Additional logic for starting sprint can be added here.
        }
        public void HandleStopSprintInput()
        {
            // Process the stop sprint input.
            InputSprint = false;
            // Additional logic for stopping sprint can be added here.
        }
        #endregion

        #region Camera Info Provider
        private IDCameraInfoProvider _cameraInfo;

        public void SetCameraInfoProvider(IDCameraInfoProvider cameraInfoProvider)
        {
            _cameraInfo = cameraInfoProvider;
        }

        #endregion

        #region Movement Methods
        private const float _speedOffset = 0.1f;
        public float TargetSpeed { get; set; }  // State에서 설정

        

        public void InputMoveSpeed()
        {
            if (InputSprint)
            {
                TargetSpeed = MovementProfile.SprintSpeed;
            }
            else if (HasMovementInput)
            {
                TargetSpeed = MovementProfile.MoveSpeed;
            }
            else
            {
                TargetSpeed = 0f; // No movement input, set speed to zero
            }
        }


        public void CalculateGroundSpeed()
        {
            float currentHorizontalSpeed = new Vector3(_characterController.velocity.x, 0.0f, _characterController.velocity.z).magnitude;

            if (Mathf.Abs(currentHorizontalSpeed - TargetSpeed) > _speedOffset)
            {
                float desired = TargetSpeed;

                _currentSpeed = Mathf.Lerp(currentHorizontalSpeed, desired, Time.deltaTime * MovementProfile.SpeedChangeRate);
                _currentSpeed = Mathf.Round(_currentSpeed * 1000f) / 1000f;
            }
            else
            {
                _currentSpeed = TargetSpeed;
            }
        }
        public void CalculateAirSpeed()
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, MovementProfile.MoveSpeed, MovementProfile.AirDeceleration * Time.deltaTime);
        }
        public void CalculateMoveDirection()
        {
            _moveDirection = Quaternion.Euler(0f, _cameraInfo.YawAngle, 0f) * InputDirection;
            _moveDirection.Normalize();
            if (HasMovementInput)
            {
                _lastDirection = _moveDirection;
            }
        }


        public void ApplyCharacterRotation()
        {
            if (HasMovementInput)
            {
                _targetRotation = Mathf.Atan2(_lastDirection.x, _lastDirection.z) * Mathf.Rad2Deg + _cameraInfo.YawAngle;
                if (!IsForward()) _targetRotation += 180f;

                _characterRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationSpeed, MovementProfile.RotationSmoothTime);
            }

            transform.rotation = Quaternion.Euler(0f, _characterRotation, 0f);
        }

        public void ApplyMovement()
        {
            Vector3 velocity = _moveDirection * _currentSpeed + Vector3.up * _verticalSpeed;
            _characterController.Move(velocity * Time.deltaTime);
        }

        #endregion

        #region Jump Control

        
        public bool _grounded;

        public bool IsGrounded
        {
            get { return _grounded; }
            set { _grounded = value; }
        }

        public void GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _movementProfile.GroundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _movementProfile.GroundedRadius, _movementProfile.GroundLayers, QueryTriggerInteraction.Ignore);
        }
        public void ApplyGravity()
        {
            if (!_grounded)
            {
                VerticalSpeed += MovementProfile.AirborneGravity * Time.deltaTime;
            }
        }

        Coroutine jumpTimeoutCoroutine = null;
        Coroutine fallTimeoutCoroutine = null;
        public bool _isJumpReady = true;
        public bool _isFalling = false;
        public bool IsJumpReady { get => _isJumpReady; set => _isJumpReady = value; }
        public bool IsFalling { get => _isFalling; set => _isFalling = value; }

        public float VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }

        public bool CanJump() => _grounded && _isJumpReady;
        public bool ShouldJump() => InputJump && CanJump();
        public bool ShouldGrounded() => _grounded && _isFalling;


        public void PerformJump()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            _verticalSpeed = Mathf.Sqrt(MovementProfile.JumpHeight * -2f * MovementProfile.AirborneGravity);
            _isJumpReady = false;
            JumpTimeoutRoutine();
        }

        public IEnumerator JumpTimeoutRoutine()
        {
            // Wait for the jump timeout duration before allowing another jump
            yield return new WaitForSeconds(MovementProfile.JumpTimeout);
            // Reset the jump cooldown
            _isJumpReady = true;
        }

        public void ResetJump()
        {
            jumpTimeoutCoroutine = StartCoroutine(JumpTimeoutRoutine());
        }

        // 이건 굳이 사용하진 않는 기능임
        public void ResetJumpCancel()
        {
            if (jumpTimeoutCoroutine != null)
            {
                StopCoroutine(jumpTimeoutCoroutine);
                jumpTimeoutCoroutine = null;
                _isJumpReady = false;
            }
        }

        public IEnumerator FallTimeoutRoutine()
        {
            // Wait for the fall timeout duration before allowing another jump
            yield return new WaitForSeconds(MovementProfile.FallTimeout);
            _isFalling = true;
        }

        public void TryFall()
        {
            fallTimeoutCoroutine = StartCoroutine(FallTimeoutRoutine());
        }
        public void TryFallCancel()
        {
            if (fallTimeoutCoroutine != null)
            {
                StopCoroutine(fallTimeoutCoroutine);
                fallTimeoutCoroutine = null;
                _isFalling = false;
            }
        }


        #endregion
    }
}