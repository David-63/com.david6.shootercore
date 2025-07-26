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
        public IDAnimatorProvider AnimatorProvider => _animatorHandler;
        public IDCooldownProvider CooldownProvider => _cooldownHandler;


        public Transform CharacterTransform => transform;
        #region Input caching

        public Vector3 InputDirection { get; private set; }
        public bool InputJump { get; private set; }
        public bool InputSprint { get; private set; }
        public bool InputAim { get; private set; }
        public bool InputFire { get; private set; }
        public bool InputReload { get; private set; }
        public void HandleMoveInput(Vector2 moveInput) => InputDirection = new Vector3(moveInput.x, 0, moveInput.y);
        public void HandleStartJumpInput() => InputJump = true;
        public void HandleStopJumpInput() => InputJump = false;
        public void HandleStartSprintInput() => InputSprint = true;
        public void HandleStopSprintInput() => InputSprint = false;
        public void HandleStartAimInput() => InputAim = true;
        public void HandleStopAimInput() => InputAim = false;
        public void HandleStartFireInput() => InputFire = true;
        public void HandleStopFireInput() => InputFire = false;
        public void HandleStartReloadInput() => InputReload = true;
        public void HandleStopReloadInput() => InputReload = false;
        #endregion

        #region Camera Info Provider
        IDCameraInfoProvider _cameraInfo;

        public bool SetCameraInfoProvider(IDCameraInfoProvider cameraInfoProvider)
        {
            bool flag = true;
            if (cameraInfoProvider != null)
            {
                _cameraInfo = cameraInfoProvider;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        #endregion

        #region Movement Methods

        public float TargetSpeed { get; set; }
        float _horizontalSpeed;
        public float HorizontalSpeed { get => _horizontalSpeed; set => _horizontalSpeed = value; }

        Vector3 _finalMoveDirection;
        public Vector3 FinalMoveDirection { get => _finalMoveDirection; set => _finalMoveDirection = value; }
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

        public Coroutine ExecuteCoroutine(IEnumerator routine) => StartCoroutine(routine);
        public void CancelCoroutine(Coroutine routine) => StopCoroutine(routine);

        #region Jump Control


        bool _grounded = true;

        public bool IsGrounded { get => _grounded; set => _grounded = value; }

        bool _isJumpReady = true;
        public bool IsJumpReady { get => _isJumpReady; set => _isJumpReady = value; }
        bool _isFalling = false;
        public bool IsFalling { get => _isFalling; set => _isFalling = value; }

        float _verticalSpeed;
        public float VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }

        // Locomotion 조건
        public bool HasMovementInput() => InputDirection.x != 0 || InputDirection.z != 0;
        public bool IsForward() => _finalMoveDirection.z >= -0.8f;
        public bool CanJump() => _grounded && _isJumpReady;
        public bool ShouldJump() => InputJump && CanJump();

        /// <summary>
        /// Airborne 에 진입 했었고, 착지한 경우에 해당
        /// </summary>
        public bool ShouldGrounded() => _grounded && _isFalling; // 점프 보정

        #endregion

        #region Action Control

        // 임시로 구성한 변수



        // 현재 사용여부
        bool _isFiring = false;
        public bool IsFiring { get => _isFiring; set => _isFiring = value; }


        // FireRate는 임시 변수임
        public float _FireRate = 720f;
        public float FireRate => _FireRate;

        public bool ShouldFire() => InputFire && !_isFiring;

        public bool _isReloadReady = true;
        public bool IsReloadReady { get => _isReloadReady; set => _isReloadReady = value; }

        public bool CanReload() => _isReloadReady;

        public bool ShouldReload() => InputReload && CanReload();
        #endregion
    }
}