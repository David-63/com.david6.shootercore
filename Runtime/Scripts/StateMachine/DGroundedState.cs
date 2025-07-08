using David6.ShooterCore.Provider;
using David6.ShooterCore.Tools;

namespace David6.ShooterCore.StateMachine
{
    public class DGroundedState : DBaseState
    {
        public DGroundedState(IDContextProvider context, DStateFactory stateFactory)
         : base(context, stateFactory) { }
        public override void EnterState()
        {
            Log.WhatHappend("Entering Grounded State");
            // 바닥에 있는 동안 중력은 y -0.1정도로 적용됨
            _context.VerticalSpeed = _context.MovementProfile.GroundGravity;
        }

        public override void UpdateSelf()
        {
            CheckTransition();            
        }

        public override void ExitState()
        {
            // Cleanup when exiting grounded state
            _context.TryFall();
        }
        public override void CheckTransition()
        {
            if (!_context.IsGrounded || _context.ShouldJump())
            {
                SwitchState(_factory.Airborne());
            }
        }
        public override void InitializeSubState()
        {
            // Initialize any substates if needed
            // For example, if the player is crouching, set the substate to crouching
        }
    }
}