using System.Collections;
using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;
using UnityEngine;

namespace David6.ShooterCore.StateMachine
{
    public class DAirborneState : DBaseState
    {
        public DAirborneState(IDContextProvider context, DStateFactory stateFactory)
            : base(context, stateFactory)
        {
        }

        public override void EnterState()
        {
            HandleJump();

        }

        public override void UpdateSelf()
        {
            CheckTransition();
            _context.ApplyGravity();
            //HandleGravity();
        }

        public override void ExitState()
        {
            // Cleanup when exiting airborne state
            _context.ResetJump();

        }
        public override void CheckTransition()
        {
            // Check conditions to transition to another state
            // For example, if the player lands, transition to grounded state

            // Ground 조건 체크
            if (_context.ShouldGrounded())
            {
                _context.IsFalling = false;
                SwitchState(_factory.Grounded());
            }
        }
        public override void InitializeSubState()
        {
            // Initialize any substates if needed
            // For example, if the player is falling, set the substate to falling
        }

        void HandleJump()
        {
            // 조건 구분?
            if (_context.ShouldJump())
            {
                Log.WhatHappend("Jump로 Airborne 진입");
                _context.PerformJump();
            }
            else
            {
                Log.WhatHappend("Fall로 Airborne 진입");
            }
        }
    }
}