using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine
{
    public class DAirborneState : DBaseState
    {
        public DAirborneState(IDContextProvider context, IDStateMachineProvider stateMachine)
         : base(context, stateMachine)
        {
            IsRoot = true;
        }

        public override void EnterState()
        {
            HandleJump();
        }

        public override void UpdateSelf()
        {
            CheckTransition();
            Context.ApplyGravity();
        }

        public override void ExitState()
        {
            Context.IsFalling = false;
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
        }

        void HandleJump()
        {
            // 조건 구분?
            if (Context.ShouldJump())
            {
                Log.WhatHappend("Airborne: Jump");
                TryJump();
            }
            else
            {
                Log.WhatHappend("Airborne: FreeFall");
            }
        }

        public void TryJump()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            Context.VerticalSpeed = Mathf.Sqrt(Context.MovementProfile.JumpHeight * -2f * Context.MovementProfile.AirborneGravity);
            Context.IsJumpReady = false;
            Context.ResetJump();
        }
    }
}