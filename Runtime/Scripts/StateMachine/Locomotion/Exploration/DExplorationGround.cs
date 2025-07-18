using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine.Locomotion
{
    public abstract class DExplorationGround : DBaseState
    {
        private const float _speedOffset = 0.1f;
        private float _rotationSpeed;

        public DExplorationGround(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine) {}

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