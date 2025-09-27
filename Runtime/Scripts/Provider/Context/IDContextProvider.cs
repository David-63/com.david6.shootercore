using System.Collections;
using David6.ShooterCore.Data;
using David6.ShooterCore.Item;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDContextProvider : IDProvider
    {
        CharacterController Controller { get; }
        DMovementProfile MovementProfile { get; }
        Transform CharacterTransform { get; }
        Transform WeaponSocket { get; }

        IDAnimatorHandlerProvider AnimatorProvider { get; }
        IDCooldownProvider CooldownProvider { get; }
        IDCameraHandlerProvider CameraHandlerProvider { get; }
        IDRigHandlerProvider RigHandlerProvider { get; }
        IDCombatHandler CombatHandler { get; }

        bool SetCameraHandler(IDCameraHandlerProvider cameraHandler);
        bool SetEquipmentUIController(IDEquipmentUIControllerProvider uiController);
        bool SetFocusUIController(IDFocusUIControllerProvider uiController);
        
        bool SetRigHandler(IDRigHandlerProvider rigHandler);

        /// <summary>
        ///  debug 모드 활성화
        /// </summary>
        void ActiveStateDebugMode();

        #region Input
        Vector3 InputDirection { get; }
        bool InputSprint { get; }
        bool InputJump { get; }
        bool InputAim { get; }
        bool InputFire { get; }

        void HandleMoveInput(Vector2 moveInput);
        void HandleStartJumpInput();
        void HandleStopJumpInput();
        void HandleStartSprintInput();
        void HandleStopSprintInput();
        void HandleStartAimInput();
        void HandleStopAimInput();
        void HandleStartFireInput();
        void HandleStopFireInput();
        void HandleStartReloadInput();
        void HandleStopReloadInput();


        // UI 입력
        void HandlePauseInput();
        void HandleResumeInput();
        void HandleCancelInput();

        #endregion

        // 이벤트 바인딩
        void HandleCloseUI();
        void OnGearEquipped(DGear data);

        // Movement 변수
        float HorizontalSpeed { get; set; }
        float TargetSpeed { get; set; }
        Vector3 FinalMoveDirection { get; set; }
        Vector2 CaptureDirection { get; set; }
        float YawAngle { get; }
        float VerticalSpeed { get; set; }
        bool IsGrounded { get; }
        bool IsJumpReady { get; set; }
        bool IsFalling { get; set; }


        // 코루틴 호출함수
        Coroutine ExecuteCoroutine(IEnumerator routine);
        void CancelCoroutine(Coroutine routine);

        // 조건
        bool HasMovementInput();
        bool IsForward();
        bool CanJump();
        bool ShouldJump();
        bool ShouldGrounded();

        bool IsTriggerReleased { get; set; }
        int AmmunitionCount(EDAmmoType type);
        int ConsumeAmmunition(EDAmmoType type, int capacity);

        bool ShouldFire();
        bool ShouldReload();

        void FireRoundRumble();
        void EmptyChamberRumble();

        void EjectRumble();
        void InsertRumble();
        void ChamberLoadRumble();
        void CancelRumble();

        void StopRumble(float delay);

        GameObject MakeObject(GameObject prefab, Transform target);
        GameObject MakeObject(GameObject prefab, Vector3 position, Vector3 normal);

        GameObject AssembleWeapon(DGear gear, Transform socket);

        GameObject SpawnParticle(GameObject vfxPrefab, Vector3 position, Quaternion rotation, Transform parent = null);
        GameObject SpawnTrail(GameObject vfxPrefab, Vector3 position, Quaternion rotation, Transform parent = null);
        void DestroyPrefab(GameObject target);


    }
}