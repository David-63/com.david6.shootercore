using David6.ShooterCore.Movement;
using David6.ShooterCore.StateMachine;
using UnityEngine;

namespace David6.ShooterCore.Provider
{
    public interface IDContextProvider
    {
        DMovementProfile MovementProfile { get; }

        #region Input
        Vector3 InputDirection { get; }
        Vector2 InputLook { get; }
        bool InputSprint { get; }
        bool InputJump { get; }

        void HandleMoveInput(Vector2 moveInput);
        //void HandleLookInput(Vector2 lookInput);
        void HandleStartJumpInput();
        void HandleStopJumpInput();
        void HandleStartSprintInput();
        void HandleStopSprintInput();

        bool HasMovementInput { get; }
        #endregion

        void SetCameraInfoProvider(IDCameraInfoProvider cameraInfoProvider);

        float TargetSpeed { get; set; }
        float VerticalSpeed { get; set; }
        bool IsGrounded { get; }
        bool IsJumpReady { get; set; }
        bool IsFalling { get; set; }

        void GroundCheck();
        void ApplyGravity();

        void InputMoveSpeed();
        void ApplyMovement();


        void PerformJump();

        // 조건
        bool CanJump();
        bool ShouldJump();
        bool ShouldGrounded();

        // 코루틴 호출함수
        void ResetJump();
        void ResetJumpCancel();
        void TryFall();
        void TryFallCancel();
    }
}