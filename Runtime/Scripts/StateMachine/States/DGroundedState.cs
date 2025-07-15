using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine
{
    public class DGroundedState : DBaseState
    {
        private const float _speedOffset = 0.1f;
        private float _rotationSpeed;

        public DGroundedState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine)
        {
            IsRoot = true;
        }
        public override void EnterState()
        {
            InitializeSubState();
            Context.VerticalSpeed = Context.MovementProfile.GroundGravity;
        }

        public override void UpdateSelf()
        {
            CheckTransition();
            GroundSpeed();
            MoveDirection();
            ApplyCharacterRotation();
        }

        public override void ExitState()
        {
            // Cleanup when exiting grounded state
            Context.IsFalling = false;
        }
        public override void CheckTransition()
        {
            if (!Context.IsGrounded || Context.ShouldJump())
            {
                SwitchState(StateMachine.Factory.Airborne());
            }
        }
        public override void InitializeSubState()
        {
            if (!Context.HasMovementInput())
            {
                SetSubState(StateMachine.Factory.Idle());
            }
            else if (!Context.InputSprint)
            {
                SetSubState(StateMachine.Factory.Walk());
            }
            else
            {
                SetSubState(StateMachine.Factory.Run());
            }

            if (SubState != null)
            {
                Log.WhatHappend($"[SubState Enter] {SubState.GetType().Name}");
                SubState.EnterState();
            }
        }

        void GroundSpeed()
        {
            if (Mathf.Abs(Context.HorizontalSpeed - Context.TargetSpeed) > _speedOffset)
            {
                Context.HorizontalSpeed = Mathf.Lerp(Context.HorizontalSpeed, Context.TargetSpeed, Time.deltaTime * Context.MovementProfile.SpeedChangeRate);
                Context.HorizontalSpeed = Mathf.Round(Context.HorizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                Context.HorizontalSpeed = Context.TargetSpeed;
            }
        }

        void MoveDirection()
        {
            Vector3 moveDirection = Quaternion.Euler(0f, Context.YawAngle, 0f) * Context.InputDirection;
            moveDirection.Normalize();
            if (Context.HasMovementInput())
            {
                Context.FinalMoveDirection = moveDirection;
            }
        }

        private void ApplyCharacterRotation()
        {
            if (Context.HasMovementInput())
            {
                // 벡터의 x, z 성분을 사용하여 해당 방향이 z축의 +방향으로부터 몇 도 떨어저 있는지 계산
                float targetRotation = Mathf.Atan2(Context.InputDirection.x, Context.InputDirection.z) * Mathf.Rad2Deg + Context.YawAngle;
                float calcRotation = Mathf.SmoothDampAngle(Context.CharacterTransform.eulerAngles.y, targetRotation, ref _rotationSpeed, Context.MovementProfile.RotationSmoothTime);
                Context.CharacterTransform.rotation = Quaternion.Euler(0.0f, calcRotation, 0.0f);
            }
        }
    }
}