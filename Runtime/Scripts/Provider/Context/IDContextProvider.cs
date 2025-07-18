using System.Collections;
using David6.ShooterCore.Movement;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDContextProvider
    {
        DMovementProfile MovementProfile { get; }
        Transform CharacterTransform { get; }
        IDAnimatorProvider AnimatorProvider { get; }


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

        #endregion

        void SetCameraInfoProvider(IDCameraInfoProvider cameraInfoProvider);

        float HorizontalSpeed { get; set; }
        float TargetSpeed { get; set; }
        Vector3 FinalMoveDirection { get; set; }
        float YawAngle { get; }
        float VerticalSpeed { get; set; }
        bool IsGrounded { get; }
        bool IsJumpReady { get; set; }
        bool IsFalling { get; set; }

        // Update
        void GroundCheck();
        void ApplyMovement();

        // 조건
        bool HasMovementInput();
        bool IsForward();
        bool CanJump();
        bool ShouldJump();
        bool ShouldGrounded();

        // 코루틴 호출함수
        Coroutine ExecuteCoroutine(IEnumerator routine);
        void CancelCoroutine(Coroutine routine);
    }
}