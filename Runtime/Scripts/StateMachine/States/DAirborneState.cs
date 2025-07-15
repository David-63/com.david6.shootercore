using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine
{
    public class DAirborneState : DBaseState
    {
        Coroutine _jumpTimeoutCoroutine;
        Coroutine _fallTimeoutCoroutine;
        public DAirborneState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine)
        {
            IsRoot = true;
        }

        public override void EnterState()
        {
            if (Context.ShouldJump())
            {
                Context.AnimatorProvider.SetFreeFall(false);
                Context.AnimatorProvider.SetJump(true);
                TryJump();
            }
            else
            {
                Context.AnimatorProvider.SetFreeFall(true);
                Context.AnimatorProvider.SetJump(false);
            }
            StartFallDelay();
            Context.AnimatorProvider.SetGrounded(false);
        }

        public override void UpdateSelf()
        {
            CheckTransition();
            ApplyGravity();
        }

        public override void ExitState()
        {
            Context.IsFalling = false;
            Context.AnimatorProvider.SetFreeFall(false);
            Context.AnimatorProvider.SetGrounded(true);
        }
        public override void CheckTransition()
        {
            if (Context.ShouldGrounded())
            {
                SwitchState(StateMachine.Factory.Grounded());
            }
        }
        public override void InitializeSubState()
        {
            // 필요해지면 fall jump 만들기
        }

        void TryJump()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            Context.VerticalSpeed = Mathf.Sqrt(Context.MovementProfile.JumpHeight * -2f * Context.MovementProfile.AirborneGravity);
            Context.IsJumpReady = false;
            StartJumpCooldown();
        }
        void ApplyGravity()
        {
            if (!Context.IsGrounded)
            {
                Context.VerticalSpeed += Context.MovementProfile.AirborneGravity * Time.deltaTime;
            }
        }
        IEnumerator JumpTimeoutRoutine()
        {
            // Wait for the jump timeout duration before allowing another jump
            yield return new WaitForSeconds(Context.MovementProfile.JumpTimeout);
            // Reset the jump cooldown
            Context.IsJumpReady = true;
            Context.AnimatorProvider.SetJump(false);
        }
        void StartJumpCooldown()
        {
            _jumpTimeoutCoroutine = Context.ExecuteCoroutine(JumpTimeoutRoutine());
        }
        void CancelJumpCooldown()
        {
            if (_jumpTimeoutCoroutine != null)
            {
                Context.CancelCoroutine(_jumpTimeoutCoroutine);
                _jumpTimeoutCoroutine = null;
                Context.IsJumpReady = false;
            }
        }
        IEnumerator FallTimeoutRoutine()
        {
            // Wait for the fall timeout duration before allowing another jump
            yield return new WaitForSeconds(Context.MovementProfile.FallTimeout);
            Context.IsFalling = true;
            Context.AnimatorProvider.SetFreeFall(true);
        }
        void StartFallDelay()
        {
            _fallTimeoutCoroutine = Context.ExecuteCoroutine(FallTimeoutRoutine());
        }

        void CancelFallDelay()
        {
            if (_fallTimeoutCoroutine != null)
            {
                Context.CancelCoroutine(_fallTimeoutCoroutine);
                _fallTimeoutCoroutine = null;
                Context.IsFalling = false;
            }
        }
    }
}