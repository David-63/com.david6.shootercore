using System.Collections;
using David6.ShooterCore.Data;
using David6.ShooterCore.Data.Enum;
using David6.ShooterCore.Data.Gear;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDContextProvider : IDProvider
    {
        DMovementProfile MovementProfile { get; }
        Transform CharacterTransform { get; }
        Transform WeaponSocket { get; }
        CharacterController Controller { get; }

        IDAnimatorHandlerProvider AnimatorProvider { get; }
        IDCooldownProvider CooldownProvider { get; }
        IDCameraHandlerProvider CameraHandlerProvider { get; }
        IDRigHandlerProvider RigHandlerProvider { get; }
        IDCombatHandler CombatHandler { get; }

        bool SetCameraHandler(IDCameraHandlerProvider cameraHandler);
        bool SetRootPanelController(IDRootPanelControllerProvider rootPanelController);
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
        void HandlePopInput();

        #endregion

        // 이벤트 바인딩
        void HandleCloseUI();
        void OnGearEquipped(EDGearType type, DGearData data);

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

        bool IsFocus { get; set; }

        void StartFocus();
        void RequestCameraTransition(EDCameraType camera);


        bool IsFiring { get; set; }
        float FireRate { get; }
        bool ShouldFire();
        bool ShouldReload();

        GameObject MakeObject(GameObject prefab, Transform target);
        GameObject MakeObject(GameObject prefab, Vector3 position, Vector3 normal);


    }
}