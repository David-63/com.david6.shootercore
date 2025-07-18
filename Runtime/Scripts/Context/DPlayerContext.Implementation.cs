using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Context
{
    /// <summary>
    /// Represents the player's movement component.
    /// </summary>
    public partial class DPlayerContext : MonoBehaviour, IDContextProvider
    {
        public IDAnimatorProvider AnimatorProvider {get { return _animatorProvider; } }

        public Transform CharacterTransform { get { return transform; } }
        #region Input caching

        public Vector3 InputDirection { get; private set; }
        public bool InputJump { get; private set; }
        public bool InputSprint { get; private set; }
        public bool InputAim { get; private set; }
        public bool InputFire { get; private set; }
        public void HandleMoveInput(Vector2 moveInput) => InputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        public void HandleStartJumpInput() => InputJump = true;
        public void HandleStopJumpInput() => InputJump = false;
        public void HandleStartSprintInput() => InputSprint = true;
        public void HandleStopSprintInput() => InputSprint = false;
        public void HandleStartAimInput() => InputAim = true;
        public void HandleStopAimInput() => InputAim = false;
        public void HandleStartFireInput() => InputFire = true;
        public void HandleStopFireInput() => InputFire = false;
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

        
        bool _grounded = true;

        public bool IsGrounded
        {
            get { return _grounded; }
            set { _grounded = value; }
        }

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
        public bool ShouldGrounded() => _grounded && _isFalling; // 점프 보정

        public Coroutine ExecuteCoroutine(IEnumerator routine) => StartCoroutine(routine);
        public void CancelCoroutine(Coroutine routine) => StopCoroutine(routine);

        #endregion
    }
}