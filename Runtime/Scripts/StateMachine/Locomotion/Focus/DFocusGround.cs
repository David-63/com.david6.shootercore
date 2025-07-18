using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public abstract class DFocusGround : DBaseState
    {
        private const float _speedOffset = 0.1f;
        private float _rotationSpeed;
        private float _characterRotation;
        private float _rotationVelocity;

        public DFocusGround(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) { }

        protected void GroundSpeed()
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

        protected void MoveDirection()
        {
            Vector3 moveDirection = Quaternion.Euler(0f, Context.YawAngle, 0f) * Context.InputDirection;
            moveDirection.Normalize();
            if (Context.HasMovementInput())
            {
                Context.FinalMoveDirection = moveDirection;
            }
        }

        protected void ApplyCharacterRotation()
        {
            _characterRotation = Mathf.SmoothDampAngle(Context.CharacterTransform.eulerAngles.y, Context.YawAngle, ref _rotationSpeed, Context.MovementProfile.RotationSmoothTime);
            Context.CharacterTransform.rotation = Quaternion.Euler(0f, _characterRotation, 0f);
        }
    }
}