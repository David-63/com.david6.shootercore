using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public abstract class DFocusGround : DBaseState
    {
        private const float _speedOffset = 0.05f;
        private float _rotationSpeed;
        private float _characterRotation;

        protected float _focusMovementMultipler = 0.8f;

        public DFocusGround(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        protected void GroundSpeed(float deltaTime)
        {
            if (Mathf.Abs(Context.HorizontalSpeed - Context.TargetSpeed) > _speedOffset)
            {
                Context.HorizontalSpeed = Mathf.Lerp(Context.HorizontalSpeed, Context.TargetSpeed, deltaTime * Context.MovementProfile.SpeedChangeRate);
                Context.HorizontalSpeed = Mathf.Round(Context.HorizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                Context.HorizontalSpeed = Context.TargetSpeed;
            }
            Context.AnimatorProvider.SetSpeed(Context.HorizontalSpeed);
        }

        protected void MoveDirection()
        {
            Vector3 targetDirection = Vector3.zero;

            if (Context.HasMovementInput())
            {
                targetDirection = Quaternion.Euler(0.0f, Context.YawAngle, 0.0f) * Context.InputDirection;
                targetDirection.Normalize();
            }
            Context.FinalMoveDirection = targetDirection;
        }

        protected void ApplyCharacterRotation()
        {
            _characterRotation = Mathf.SmoothDampAngle(Context.CharacterTransform.eulerAngles.y, Context.YawAngle, ref _rotationSpeed, Context.MovementProfile.RotationSmoothTime);
            Context.CharacterTransform.rotation = Quaternion.Euler(0f, _characterRotation, 0f);
        }

        protected void SetAnimationDirection(float deltaTime)
        {
            // 입력 방향 (카메라 기준 변환 가능)
            // Quaternion.Euler(0f, Context.YawAngle, 0f) * 
            Vector3 rawDirection = Context.InputDirection;
            rawDirection.Normalize();
            // TargetSpeed 비율 (RunSpeed = 1 기준)
            float speedRatio = Context.TargetSpeed / Context.MovementProfile.RunSpeed;
            //Log.WhatHappend(speedRatio);
            Vector2 targetAnimDir = new Vector2(rawDirection.x, rawDirection.z) * speedRatio;
            // 보간된 방향 (애니메이션 전용)
            Context.CaptureDirection = Vector2.Lerp(Context.CaptureDirection, targetAnimDir, deltaTime * Context.MovementProfile.SpeedChangeRate);

            Context.AnimatorProvider.SetDirection(Context.CaptureDirection);
        }
    }
}