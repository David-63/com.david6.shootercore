using System.Collections;
using David6.ShooterCore.Data;
using David6.ShooterCore.Data.Enum;
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
        public DMovementProfile MovementProfile => _movementProfile;
        public IDAnimatorProvider AnimatorProvider => _animatorHandler;
        public IDCooldownProvider CooldownProvider => _cooldownHandler;
        public IDCameraHandlerProvider CameraHandlerProvider => _cameraHandler;

        public Transform CharacterTransform => transform;


        public void ActiveStateDebugMode()
        {
            _locomotionStateMachine.ActiveStateDebugMode();
            _actionStateMachine.ActiveStateDebugMode();
        }

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


        public IDRootPanelControllerProvider RootPanelController => _rootPanelController;

        public void HandlePauseInput()
        {
            // focus 해제
            _cooldownHandler.CancelCooldown(FOCUS_KEY);
            IsFocus = false;

            // 카메라 변경
            RequestCameraTransition(EDCameraType.Pause);

            // ui 활성화
            _rootPanelController.HandlePause();
        }
        public void HandleResumeInput()
        {
            _rootPanelController.HandleResume();
        }
        public void HandlePopInput()
        {
            _rootPanelController.HandlePop();
        }

        #endregion

        public void HandleCloseUI()
        {
            RequestCameraTransition(EDCameraType.Exploration);
        }

        public bool SetCameraInfoProvider(IDCameraHandlerProvider cameraInfoProvider)
        {
            bool flag = true;
            if (cameraInfoProvider != null)
            {
                _cameraHandler = cameraInfoProvider;
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        public bool SetRootPanelController(IDRootPanelControllerProvider rootPanelController)
        {
            bool flag = true;
            if (rootPanelController != null)
            {
                _rootPanelController = rootPanelController;
            }
            else
            {
                flag = false;
            }
            return flag;
        }


        #region Movement
        public float TargetSpeed { get; set; }
        float _horizontalSpeed;
        public float HorizontalSpeed { get => _horizontalSpeed; set => _horizontalSpeed = value; }

        Vector3 _finalMoveDirection;
        public Vector3 FinalMoveDirection { get => _finalMoveDirection; set => _finalMoveDirection = value; }
        public float YawAngle => _cameraHandler.YawAngle;


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

        bool _isFocus = false;
        const string FOCUS_KEY = "State.Focus";
        float _focusDuration = 5.0f;

        public bool IsFocus
        {
            get
            {
                return _isFocus;
            }
            set
            {
                _isFocus = value;
                AnimatorProvider.SetFocus(_isFocus);
                if (_isFocus)
                {
                    RequestCameraTransition(EDCameraType.Focus);
                }
                else
                {
                    RequestCameraTransition(EDCameraType.Exploration);
                }
            }
        }

        public void StartFocus()
        {
            _cooldownHandler.StartCooldown(FOCUS_KEY, _focusDuration);
            IsFocus = true;
        }

        // 아래의 변수는 전부 Inventory Module에 의해 제어될 예정
        bool _isFiring = false;
        public bool IsFiring { get => _isFiring; set => _isFiring = value; }


        // FireRate는 임시 변수임
        public float _FireRate = 720f;
        public float FireRate => _FireRate;

        public bool ShouldFire() => InputFire && !_isFiring;

        public bool ShouldReload() => InputReload;


        // 카메라 전환 요청
        bool _cameraSwapFlag = false;
        EDCameraType _cameraStateType = EDCameraType.None;

        /// <summary>
        /// 카메라 변경은 이 함수를 통해서만 가능
        /// </summary>
        /// <param name="camera"></param>
        public void RequestCameraTransition(EDCameraType camera)
        {
            _cameraSwapFlag = true;
            _cameraStateType = camera;
        }
        #endregion

        #region Tick Method

        void GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _movementProfile.GroundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _movementProfile.GroundedRadius, _movementProfile.GroundLayers, QueryTriggerInteraction.Ignore);
        }
        void ApplyMovement()
        {
            Vector3 velocity = _finalMoveDirection * _horizontalSpeed + Vector3.up * _verticalSpeed;
            _characterController.Move(velocity * Time.deltaTime);
        }
        void FocusCheck()
        {
            if (!IsFocus) return;

            if (_cooldownHandler.IsReady(FOCUS_KEY))
            {
                IsFocus = false;
            }
        }
        void CameraTransitionCheck()
        {
            if (!_cameraSwapFlag) return;

            _cameraHandler.ActivateCamera(_cameraStateType);
        }


        #endregion
    }
    /*

        Focus 관련해서 전용 클래스를 하나 만들어야할 것 같음
        이것과 연관된 코드 로직도 많기 때문에, Context 내부에서 Focus 관련 로직을 구성하면 복잡하게 꼬여버리는 듯 함

    */
}