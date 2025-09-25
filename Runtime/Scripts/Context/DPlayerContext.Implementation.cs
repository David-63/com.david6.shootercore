using System.Collections;
using System.Collections.Generic;
using David6.ShooterCore.Data;
using David6.ShooterCore.Item;
using David6.ShooterCore.Item.Weapon;
using David6.ShooterCore.Look;
using David6.ShooterCore.Pool;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.Context
{
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

        public Dictionary<EDGearSlot, GameObject> EquipmentGear = new();


        #region Setup
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
        public bool SetEquipmentUIController(IDEquipmentUIControllerProvider uiControllerProvider)
        {
            bool flag = false;

            if (uiControllerProvider != null)
            {
                _equipmentUIController = uiControllerProvider;
                flag = true;
            }

            return flag;
        }
        public bool SetFocusUIController(IDFocusUIControllerProvider uiControllerProvider)
        {
            bool flag = false;

            if (uiControllerProvider != null)
            {
                _focusUIController = uiControllerProvider;
                flag = true;
            }

            return flag;
        }

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

        #endregion

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
        public void HandleStartAimInput()
        {
            InputAim = true;
            _combatHandler.RequestFocus();
            _combatHandler.LockFocus();
            _cameraHandler.SetLayerActive(EDCameraLayer.Aim, true);
        }
        public void HandleStopAimInput()
        {
            InputAim = false;
            _combatHandler.UnlockFocus();
            _cameraHandler.SetLayerActive(EDCameraLayer.Aim, false);
        }
        public void HandleStartFireInput() => InputFire = true;
        public void HandleStopFireInput() => InputFire = false;
        public void HandleStartReloadInput() => InputReload = true;
        public void HandleStopReloadInput() => InputReload = false;


        public IDEquipmentUIControllerProvider EquipmentUIController => _equipmentUIController;
        public IDFocusUIControllerProvider FocusUIController => _focusUIController;

        public void HandlePauseInput()
        {
            if (_combatHandler.IsFocus) _combatHandler.CancelFocus();

            _equipmentUIController.HandlePause();
            _cameraHandler.SetLayerActive(EDCameraLayer.Pause, true);
        }
        public void HandleResumeInput()
        {
            _equipmentUIController.HandleResume();
        }
        public void HandleCancelInput()
        {
            _equipmentUIController.HandleCancel();
        }

        #endregion

        #region Events
        public void HandleCloseUI()
        {
            _cameraHandler.SetLayerActive(EDCameraLayer.Pause, false);
        }
        public void OnGearEquipped(DGear data)
        {
            var gearType = data.GearSlot;
            _combatHandler.EquipWeapon(gearType, data);
            _rigHandler.SetupRigIK(data.GetModule<DFrameModule>().BasePrefab.GetComponent<DFrameHandler>());
        }
        #endregion



        #region Movement
        public float TargetSpeed { get; set; }
        float _horizontalSpeed;
        public float HorizontalSpeed
        {
            get => _horizontalSpeed;
            set => _horizontalSpeed = value;
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

        //public bool IsAiming() => InputAim;

        bool _isTriggerReleased = true;
        public bool IsTriggerReleased { get => _isTriggerReleased; set => _isTriggerReleased = value; }

        public bool ShouldFire() => InputFire && IsTriggerReleased;
        public bool ShouldReload() => InputReload;

        // fire
        public void FireRoundRumble() => GameInputWrapper.SetVibration(0f, 0f, 0.05f, 0.05f);
        public void EmptyChamberRumble() => GameInputWrapper.SetVibration(0f, 0f, 0.05f, 0.05f);
        // reload
        public void EjectRumble() => GameInputWrapper.SetVibration(0.0f, 0.0f, 0.0f, 0f);
        public void InsertRumble() => GameInputWrapper.SetVibration(0.2f, 0.2f, 0f, 0f);
        public void ChamberLoadRumble() => GameInputWrapper.SetVibration(0.2f, 0.2f, 0.2f, 0.2f);
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

        #endregion

        #region Create
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

        public void DestroyPrefab(GameObject target)
        {
            Destroy(target);
        }

        #endregion

        #region Equipment
        public GameObject AssembleWeapon(DGear gear, Transform socket)
        {
            if (gear == null) return null;
            var slot = gear.GearSlot;

            UnEquip(slot);

            // if (EquipmentGear.TryGetValue(gear.GearSlot, out var existing) && existing != null)
            // {
            //     Destroy(existing);
            //     EquipmentGear[slot] = null;
            // }

            // Frame 구성
            GameObject frameObject = Instantiate(gear.GetModule<DFrameModule>().BasePrefab, socket);
            DFrameHandler frameScript = frameObject.GetComponent<DFrameHandler>();

            EquipmentGear[slot] = frameObject;


            // 나머지 모듈 구성
            DMuzzleModule muzzleModule = gear.GetModule<DMuzzleModule>();
            if (muzzleModule != null)
            {
                frameScript.AttachMuzzle(muzzleModule);
                Instantiate(muzzleModule.BasePrefab, frameScript.MuzzleSocket);
            }

            DMagazineModule magazineModule = gear.GetModule<DMagazineModule>();
            if (magazineModule != null)
            {
                frameScript.AttachMagazine(magazineModule);
                Instantiate(magazineModule.BasePrefab, frameScript.MagazineSocket);
            }

            return frameObject;
        }

        public GameObject GetEquipped(EDGearSlot slot)
        {
            EquipmentGear.TryGetValue(slot, out var gear);
            return gear;
        }
        public void UnEquip(EDGearSlot slot)
        {
            if (EquipmentGear.TryGetValue(slot, out var gear) && gear != null)
            {
                Destroy(gear);
                EquipmentGear[slot] = null;
            }
        }
        
        #endregion
    }
}