using System.Collections;
using David6.ShooterCore.Data;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using David6.ShooterCore.Pool;
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
        public IDAnimatorHandlerProvider AnimatorProvider => _animatorHandler;
        public IDCooldownProvider CooldownProvider => _cooldownHandler;
        public IDCameraHandlerProvider CameraHandlerProvider => _cameraHandler;
        public IDRigHandlerProvider RigHandlerProvider => _rigHandler;
        public IDCombatHandler CombatHandler => _combatHandler;

        public CharacterController Controller => _characterController;
        public DMovementProfile MovementProfile => _movementProfile;
        public Transform CharacterTransform => transform;
        public Transform WeaponSocket => _weaponSocket;

        [SerializeField] Transform VFXHolder;

        public void ActiveStateDebugMode()
        {
            _locomotionStateMachine.ActiveStateDebugMode();
            _actionStateMachine.ActiveStateDebugMode();
        }


        public bool SetCameraHandler(IDCameraHandlerProvider cameraInfoProvider)
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

        #region Events
        public void HandleCloseUI()
        {
            RequestCameraTransition(EDCameraType.Exploration);
        }
        public void OnGearEquipped(EDGearType type, DGearData data)
        {
            _combatHandler.SetWeapon(type, data);
            _rigHandler.SetupRigIK(data.GearPrefab.GetComponent<DWeaponFrame>());
        }
        #endregion



        public bool SetRigHandler(IDRigHandlerProvider rigHandler)
        {
            bool flag = true;
            if (rigHandler != null)
            {
                _rigHandler = rigHandler;
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
        public float HorizontalSpeed
        {
            get => _horizontalSpeed;
            set
            {
                _horizontalSpeed = value;
                //Log.WhatHappend("_horizontalSpeed 세팅되는 값: "+ value);
            }
        }

        Vector3 _finalMoveDirection;
        public Vector3 FinalMoveDirection { get => _finalMoveDirection; set => _finalMoveDirection = value; }

        public Vector2 CaptureDirection { get; set; }



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

        bool _isTriggerReleased = true;
        public bool IsTriggerReleased { get => _isTriggerReleased; set => _isTriggerReleased = value; }

        public bool ShouldFire() => InputFire && IsTriggerReleased;
        public bool ShouldReload() => InputReload;

        // fire
        public void FireRoundRumble() => GameInputWrapper.SetVibration(0f, 0f, 0.2f, 0.3f);
        public void EmptyChamberRumble() => GameInputWrapper.SetVibration(0f, 0f, 0f, 0.4f);
        // reload
        public void EjectRumble() => GameInputWrapper.SetVibration(0f, 1f, 0f, 0f);
        public void InsertRumble() => GameInputWrapper.SetVibration(1f, 0f, 0f, 0f);
        public void ChamberLoadRumble() => GameInputWrapper.SetVibration(0f, 0f, 0.5f, 0f);
        public void CancelRumble() => GameInputWrapper.SetVibration(0f, 0f, 0f, 0f);

        public void StopRumble(float delay) => ExecuteCoroutine(StopSetMotorRumble(delay));

        IEnumerator StopSetMotorRumble(float delay)
        {
            yield return new WaitForSeconds(delay);

            GameInputWrapper.SetVibration(0f, 0f, 0f, 0f);

            // var gamepad = Gamepad.current;
            // if (gamepad == null)
            //     yield break;

            // gamepad.SetMotorSpeeds(0f, 0f);
        }


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

        public GameObject MakeObject(GameObject prefab, Transform target)
        {
            return Instantiate(prefab, target);
        }
        public GameObject MakeObject(GameObject prefab, Vector3 position, Vector3 normal)
        {
            return Instantiate(prefab, position, Quaternion.LookRotation(normal));
        }

        public GameObject SpawnParticle(GameObject vfxPrefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject vfx = DGamePool.Instance.Get(vfxPrefab, VFXHolder);
            vfx.transform.position = position;
            vfx.transform.rotation = rotation;

            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            return vfx;
        }
        public GameObject SpawnTrail(GameObject vfxPrefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject vfx = DGamePool.Instance.Get(vfxPrefab, VFXHolder);
            vfx.transform.position = position;
            vfx.transform.rotation = rotation;

            return vfx;
        }

    }
    /*

        Focus 관련해서 전용 클래스를 하나 만들어야할 것 같음
        이것과 연관된 코드 로직도 많기 때문에, Context 내부에서 Focus 관련 로직을 구성하면 복잡하게 꼬여버리는 듯 함

    */
}