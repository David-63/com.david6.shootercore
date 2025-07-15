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
        public IDAnimatorProvider AnimatorProvider {get { return _animatorProvider; } }

        public Transform CharacterTransform { get { return transform; } }

        public Vector3 InputDirection { get; private set; }
        public bool InputJump { get; private set; }
        public bool InputSprint { get; private set; }
        public void HandleMoveInput(Vector2 moveInput)
        {
            // Process the move input and update InputDirection accordingly.
            InputDirection = new Vector3(moveInput.x, 0, moveInput.y);
            // Additional logic for movement can be added here.
        }
        public void HandleStartJumpInput()
        {
            InputJump = true;
        }
        public void HandleStopJumpInput()
        {
            InputJump = false;
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
        IDCameraInfoProvider _cameraInfo;

        public void SetCameraInfoProvider(IDCameraInfoProvider cameraInfoProvider)
        {
            _cameraInfo = cameraInfoProvider;
        }

        #endregion

        #region Movement Methods

        float _horizontalSpeed;
        public float HorizontalSpeed { get { return _horizontalSpeed; } set { _horizontalSpeed = value; } }
        float _targetSpeed;
        public float TargetSpeed { get { return _targetSpeed; } set { _targetSpeed = value; } }

        Vector3 _finalMoveDirection;
        public Vector3 FinalMoveDirection {get{ return _finalMoveDirection; }set{ _finalMoveDirection = value; }}
        public float YawAngle => _cameraInfo.YawAngle;
        public void GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _movementProfile.GroundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _movementProfile.GroundedRadius, _movementProfile.GroundLayers, QueryTriggerInteraction.Ignore);
        }
        public void ApplyMovement()
        {
            Vector3 velocity = _finalMoveDirection * _horizontalSpeed + Vector3.up * _verticalSpeed;
            _characterController.Move(velocity * Time.deltaTime);
        }

        #endregion

        #region Jump Control

        
        bool _grounded;

        public bool IsGrounded
        {
            get { return _grounded; }
            set { _grounded = value; }
        }

        Coroutine jumpTimeoutCoroutine = null;
        Coroutine fallTimeoutCoroutine = null;
        bool _isJumpReady = true;
        public bool IsJumpReady { get => _isJumpReady; set => _isJumpReady = value; }
        bool _isFalling = false;
        public bool IsFalling { get => _isFalling; set => _isFalling = value; }

        float _verticalSpeed;
        public float VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }


        public bool HasMovementInput() => InputDirection.x != 0 || InputDirection.z != 0;
        public bool IsForward() => _finalMoveDirection.z >= -0.8f;
        public bool CanJump() => _grounded && _isJumpReady;
        public bool ShouldJump() => InputJump && CanJump();
        public bool ShouldGrounded() => _grounded && _isFalling;

        public Coroutine ExecuteCoroutine(IEnumerator routine) => StartCoroutine(routine);
        public void CancelCoroutine(Coroutine routine) => StopCoroutine(routine);

        #endregion
    }
}